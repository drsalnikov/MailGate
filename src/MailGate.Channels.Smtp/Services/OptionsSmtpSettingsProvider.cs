using MailGate.Channels.Smtp.Options;
using Microsoft.Extensions.Options;

namespace MailGate.Channels.Smtp.Services;

/// <summary>
/// Default provider that returns <see cref="SmtpOptions"/> from appsettings via <see cref="IOptionsMonitor{TOptions}"/>.
/// Used when no database override mechanism is configured.
/// </summary>
public sealed class OptionsSmtpSettingsProvider(IOptionsMonitor<SmtpOptions> options) : ISmtpSettingsProvider
{
    public Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(options.CurrentValue);
}
