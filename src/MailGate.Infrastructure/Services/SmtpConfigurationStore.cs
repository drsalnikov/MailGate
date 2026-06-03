using MailGate.Core.Entities;
using MailGate.Infrastructure.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Infrastructure.Services;

/// <summary>
/// Loads/saves the singleton SMTP override row. Transparently encrypts/decrypts secrets
/// using ASP.NET Core Data Protection so raw passwords never leave this service.
/// </summary>
public sealed class SmtpConfigurationStore(
    MailGateDbContext dbContext,
    IDataProtectionProvider dataProtectionProvider)
{
    private const string ProtectorPurpose = "MailGate.SmtpConfiguration.v1";

    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);

    public record DecryptedConfiguration(
        SmtpConfiguration Entity,
        string? Password,
        string? OAuth2ClientSecret);

    public async Task<DecryptedConfiguration?> GetAsync(CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.SmtpConfigurations.AsNoTracking()
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return null;

        var password = TryUnprotect(entity.EncryptedPassword);
        var clientSecret = TryUnprotect(entity.OAuth2EncryptedClientSecret);

        return new DecryptedConfiguration(entity, password, clientSecret);
    }

    public async Task UpsertAsync(
        SmtpConfiguration incoming,
        string? password,
        string? oauth2ClientSecret,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.SmtpConfigurations
            .FirstOrDefaultAsync(cancellationToken);

        incoming.EncryptedPassword = Protect(password) ?? existing?.EncryptedPassword;
        incoming.OAuth2EncryptedClientSecret = Protect(oauth2ClientSecret) ?? existing?.OAuth2EncryptedClientSecret;
        incoming.UpdatedAt = DateTimeOffset.UtcNow;

        if (existing is null)
        {
            dbContext.SmtpConfigurations.Add(incoming);
        }
        else
        {
            existing.Host = incoming.Host;
            existing.Port = incoming.Port;
            existing.UseSsl = incoming.UseSsl;
            existing.UseTls = incoming.UseTls;
            existing.FromAddress = incoming.FromAddress;
            existing.FromName = incoming.FromName;
            existing.Username = incoming.Username;
            existing.EncryptedPassword = incoming.EncryptedPassword;
            existing.OAuth2TenantId = incoming.OAuth2TenantId;
            existing.OAuth2ClientId = incoming.OAuth2ClientId;
            existing.OAuth2EncryptedClientSecret = incoming.OAuth2EncryptedClientSecret;
            existing.OAuth2SmtpUserPrincipalName = incoming.OAuth2SmtpUserPrincipalName;
            existing.UpdatedAt = incoming.UpdatedAt;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
    {
        var rows = await dbContext.SmtpConfigurations.ExecuteDeleteAsync(cancellationToken);
        return rows > 0;
    }

    private string? Protect(string? plain) =>
        string.IsNullOrEmpty(plain) ? null : _protector.Protect(plain);

    private string? TryUnprotect(string? encrypted)
    {
        if (string.IsNullOrEmpty(encrypted))
            return null;

        try
        {
            return _protector.Unprotect(encrypted);
        }
        catch
        {
            return null;
        }
    }
}
