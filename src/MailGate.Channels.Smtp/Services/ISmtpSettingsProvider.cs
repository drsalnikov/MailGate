using MailGate.Channels.Smtp.Options;

namespace MailGate.Channels.Smtp.Services;

/// <summary>
/// Resolves the currently effective SMTP options. Implementations may
/// combine appsettings defaults with database overrides or external config sources.
/// </summary>
public interface ISmtpSettingsProvider
{
    Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default);
}
