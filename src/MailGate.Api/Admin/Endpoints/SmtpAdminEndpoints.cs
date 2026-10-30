using System.Diagnostics;
using FluentValidation;
using MailGate.Channels.Smtp.Options;
using MailGate.Channels.Smtp.Services;
using MailGate.Contracts.Smtp;
using MailGate.Core.Entities;
using MailGate.Infrastructure.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MailGate.Api.Admin.Endpoints;

public static class SmtpAdminEndpoints
{
    public static IEndpointRouteBuilder MapSmtpAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/smtp")
            .RequireAuthorization("MailGateAdmin")
            .WithTags("Admin / SMTP");

        group.MapGet("/effective", GetEffectiveAsync).WithName("GetEffectiveSmtp");
        group.MapGet("/override", GetOverrideAsync).WithName("GetSmtpOverride");
        group.MapPut("/override", PutOverrideAsync).WithName("PutSmtpOverride");
        group.MapDelete("/override", DeleteOverrideAsync).WithName("DeleteSmtpOverride");
        group.MapPost("/test-connection", TestConnectionAsync).WithName("TestSmtpConnection");

        return app;
    }

    private static async Task<Ok<SmtpConfigurationDto>> GetEffectiveAsync(
        SmtpConfigurationStore store,
        IOptionsMonitor<SmtpOptions> appSettings,
        CancellationToken cancellationToken)
    {
        var dbConfig = await store.GetMetadataAsync(cancellationToken);

        if (dbConfig is null)
        {
            var s = appSettings.CurrentValue;
            var oauth = s.OAuth2 is null ? null : new SmtpOAuth2Dto(
                s.OAuth2.TenantId, s.OAuth2.ClientId,
                !string.IsNullOrEmpty(s.OAuth2.ClientSecret),
                s.OAuth2.SmtpUserPrincipalName);

            return TypedResults.Ok(new SmtpConfigurationDto(
                s.Host, s.Port, s.UseSsl, s.UseTls, s.CheckCertificateRevocation, s.FromAddress, s.FromName,
                s.Username, !string.IsNullOrEmpty(s.Password),
                oauth, SmtpConfigurationSource.AppSettings, null));
        }

        return TypedResults.Ok(ToDto(dbConfig, SmtpConfigurationSource.Database));
    }

    private static async Task<Results<Ok<SmtpConfigurationDto>, NotFound>> GetOverrideAsync(
        SmtpConfigurationStore store,
        CancellationToken cancellationToken)
    {
        var dbConfig = await store.GetMetadataAsync(cancellationToken);
        return dbConfig is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ToDto(dbConfig, SmtpConfigurationSource.Database));
    }

    private static async Task<Results<Ok<SmtpConfigurationDto>, ValidationProblem>> PutOverrideAsync(
        UpdateSmtpConfigurationDto dto,
        IValidator<UpdateSmtpConfigurationDto> validator,
        SmtpConfigurationStore store,
        DbSmtpSettingsProvider? provider,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var entity = new SmtpConfiguration
        {
            Host = dto.Host.Trim(),
            Port = dto.Port,
            UseSsl = dto.UseSsl,
            UseTls = dto.UseTls,
            CheckCertificateRevocation = dto.CheckCertificateRevocation,
            FromAddress = dto.FromAddress.Trim(),
            FromName = dto.FromName.Trim(),
            Username = string.IsNullOrWhiteSpace(dto.Username) ? null : dto.Username.Trim(),
            OAuth2TenantId = dto.OAuth2?.TenantId,
            OAuth2ClientId = dto.OAuth2?.ClientId,
            OAuth2SmtpUserPrincipalName = dto.OAuth2?.SmtpUserPrincipalName
        };

        await store.UpsertAsync(entity, dto.Password, dto.OAuth2?.ClientSecret, cancellationToken);
        provider?.Invalidate();

        var saved = await store.GetMetadataAsync(cancellationToken);
        return TypedResults.Ok(ToDto(saved!, SmtpConfigurationSource.Database));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOverrideAsync(
        SmtpConfigurationStore store,
        DbSmtpSettingsProvider? provider,
        CancellationToken cancellationToken)
    {
        var removed = await store.DeleteAsync(cancellationToken);
        provider?.Invalidate();
        return removed ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Ok<SmtpTestResultDto>> TestConnectionAsync(
        UpdateSmtpConfigurationDto dto,
        SmtpConfigurationStore store,
        IValidator<UpdateSmtpConfigurationDto> validator,
        IOptionsMonitor<SmtpOptions> appSettings,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.Ok(new SmtpTestResultDto(
                Success: false,
                ErrorMessage: string.Join("; ", validation.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")),
                DurationMs: 0));
        }

        var sw = Stopwatch.StartNew();
        try
        {
            var effective = BuildEffectiveOptions(dto, appSettings.CurrentValue);
            var needsSavedSecret = dto.OAuth2 is null
                ? string.IsNullOrEmpty(dto.Password)
                : string.IsNullOrEmpty(dto.OAuth2.ClientSecret);
            if (needsSavedSecret)
            {
                var saved = await store.GetMetadataAsync(cancellationToken);
                if (saved is not null)
                {
                    if (dto.OAuth2 is null)
                        effective.Password = store.DecryptPassword(saved) ?? effective.Password;
                    else if (effective.OAuth2 is not null)
                        effective.OAuth2.ClientSecret = store.DecryptOAuth2ClientSecret(saved) ?? effective.OAuth2.ClientSecret;
                }
            }
            if (effective.OAuth2 is null && !string.IsNullOrWhiteSpace(effective.Username) && string.IsNullOrEmpty(effective.Password))
                throw new InvalidOperationException("Указан SMTP-логин, но пароль отсутствует.");
            using var client = new SmtpClient();
            client.CheckCertificateRevocation = effective.CheckCertificateRevocation;
            var secureSocketOptions = effective.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : effective.UseTls
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

            await client.ConnectAsync(effective.Host, effective.Port, secureSocketOptions, cancellationToken);

            if (effective.OAuth2 is not null)
            {
                var token = await new OAuth2TokenProvider(effective).GetAccessTokenAsync(cancellationToken);
                await client.AuthenticateAsync(
                    new SaslMechanismOAuth2(effective.OAuth2.SmtpUserPrincipalName, token),
                    cancellationToken);
            }
            else if (!string.IsNullOrEmpty(effective.Username) && !string.IsNullOrEmpty(effective.Password))
            {
                await client.AuthenticateAsync(effective.Username, effective.Password, cancellationToken);
            }

            await client.DisconnectAsync(quit: true, cancellationToken);
            sw.Stop();
            return TypedResults.Ok(new SmtpTestResultDto(true, null, sw.ElapsedMilliseconds));
        }
        catch (Exception ex)
        {
            sw.Stop();
            return TypedResults.Ok(new SmtpTestResultDto(false, ex.Message, sw.ElapsedMilliseconds));
        }
    }

    private static SmtpOptions BuildEffectiveOptions(UpdateSmtpConfigurationDto dto, SmtpOptions fallback) => new()
    {
        Host = dto.Host,
        Port = dto.Port,
        UseSsl = dto.UseSsl,
        UseTls = dto.UseTls,
        CheckCertificateRevocation = dto.CheckCertificateRevocation,
        FromAddress = dto.FromAddress,
        FromName = dto.FromName,
        Username = dto.Username ?? fallback.Username,
        Password = dto.Password ?? fallback.Password,
        OAuth2 = dto.OAuth2 is null ? null : new OAuth2Options
        {
            TenantId = dto.OAuth2.TenantId,
            ClientId = dto.OAuth2.ClientId,
            ClientSecret = dto.OAuth2.ClientSecret ?? fallback.OAuth2?.ClientSecret ?? string.Empty,
            SmtpUserPrincipalName = dto.OAuth2.SmtpUserPrincipalName
        }
    };

    private static SmtpConfigurationDto ToDto(SmtpConfiguration e, SmtpConfigurationSource source)
    {
        SmtpOAuth2Dto? oauth = string.IsNullOrWhiteSpace(e.OAuth2TenantId) ? null : new SmtpOAuth2Dto(
            e.OAuth2TenantId!,
            e.OAuth2ClientId ?? string.Empty,
            !string.IsNullOrEmpty(e.OAuth2EncryptedClientSecret),
            e.OAuth2SmtpUserPrincipalName ?? string.Empty);

        return new SmtpConfigurationDto(
            e.Host, e.Port, e.UseSsl, e.UseTls, e.CheckCertificateRevocation, e.FromAddress, e.FromName,
            e.Username, !string.IsNullOrEmpty(e.EncryptedPassword),
            oauth, source, e.UpdatedAt);
    }
}
