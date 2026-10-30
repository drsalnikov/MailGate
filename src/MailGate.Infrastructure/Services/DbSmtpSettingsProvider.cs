using MailGate.Channels.Smtp.Options;
using MailGate.Channels.Smtp.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MailGate.Infrastructure.Services;

/// <summary>
/// SMTP settings provider that merges appsettings defaults with the optional database override.
/// Database values take precedence when present. Results are cached in memory for Smtp:SettingsCacheSeconds (default 30).
/// </summary>
public sealed class DbSmtpSettingsProvider(
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<SmtpOptions> appSettings,
    IMemoryCache cache) : ISmtpSettingsProvider
{
    private const string CacheKey = "MailGate.Smtp.EffectiveOptions";

    public async Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out SmtpOptions? cached) && cached is not null)
            return cached;

        var merged = await LoadAsync(cancellationToken);
        var seconds = appSettings.CurrentValue.SettingsCacheSeconds;
        if (seconds <= 0)
            throw new InvalidOperationException("Smtp:SettingsCacheSeconds must be greater than zero.");
        cache.Set(CacheKey, merged, TimeSpan.FromSeconds(seconds));
        return merged;
    }

    public void Invalidate() => cache.Remove(CacheKey);

    private async Task<SmtpOptions> LoadAsync(CancellationToken cancellationToken)
    {
        var fromSettings = appSettings.CurrentValue;

        using var scope = scopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<SmtpConfigurationStore>();
        var dbConfig = await store.GetAsync(cancellationToken);

        if (dbConfig is null)
            return Clone(fromSettings);

        var entity = dbConfig.Entity;

        var merged = new SmtpOptions
        {
            Host = entity.Host,
            Port = entity.Port,
            UseSsl = entity.UseSsl,
            UseTls = entity.UseTls,
            CheckCertificateRevocation = entity.CheckCertificateRevocation,
            FromAddress = entity.FromAddress,
            FromName = entity.FromName,
            Username = entity.Username ?? fromSettings.Username,
            Password = dbConfig.Password ?? fromSettings.Password
        };

        if (!string.IsNullOrWhiteSpace(entity.OAuth2TenantId))
        {
            merged.OAuth2 = new OAuth2Options
            {
                TenantId = entity.OAuth2TenantId ?? string.Empty,
                ClientId = entity.OAuth2ClientId ?? string.Empty,
                ClientSecret = dbConfig.OAuth2ClientSecret ?? fromSettings.OAuth2?.ClientSecret ?? string.Empty,
                SmtpUserPrincipalName = entity.OAuth2SmtpUserPrincipalName ?? string.Empty
            };
        }

        return merged;
    }

    private static SmtpOptions Clone(SmtpOptions src) => new()
    {
        Host = src.Host,
        Port = src.Port,
        UseSsl = src.UseSsl,
        UseTls = src.UseTls,
        CheckCertificateRevocation = src.CheckCertificateRevocation,
        FromAddress = src.FromAddress,
        FromName = src.FromName,
        Username = src.Username,
        Password = src.Password,
        OAuth2 = src.OAuth2 is null ? null : new OAuth2Options
        {
            TenantId = src.OAuth2.TenantId,
            ClientId = src.OAuth2.ClientId,
            ClientSecret = src.OAuth2.ClientSecret,
            SmtpUserPrincipalName = src.OAuth2.SmtpUserPrincipalName
        }
    };
}
