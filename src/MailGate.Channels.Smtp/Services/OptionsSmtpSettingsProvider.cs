using MailGate.Channels.Smtp.Options;
using Microsoft.Extensions.Options;

namespace MailGate.Channels.Smtp.Services;

public sealed class OptionsSmtpSettingsProvider(
    IOptionsMonitor<SmtpOptions> options,
    SmtpCredentialResolver credentials) : ISmtpSettingsProvider
{
    public async Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
    {
        var source = options.CurrentValue;
        var effective = new SmtpOptions
        {
            CredentialSource = source.CredentialSource,
            CredentialsFilePath = source.CredentialsFilePath,
            Host = source.Host, Port = source.Port,
            UseSsl = source.UseSsl, UseTls = source.UseTls,
            CheckCertificateRevocation = source.CheckCertificateRevocation,
            FromAddress = source.FromAddress, FromName = source.FromName,
            Username = source.Username, Password = source.Password, OAuth2 = source.OAuth2
        };
        await credentials.ApplyAsync(effective, cancellationToken);
        return effective;
    }
}
