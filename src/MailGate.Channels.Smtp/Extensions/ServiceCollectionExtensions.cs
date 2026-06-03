using MailGate.Channels.Smtp.Options;
using MailGate.Channels.Smtp.Services;
using MailGate.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MailGate.Channels.Smtp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmtpChannel(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.TryAddSingleton<ISmtpSettingsProvider, OptionsSmtpSettingsProvider>();
        services.AddSingleton<INotificationChannel, SmtpChannel>();
        return services;
    }
}
