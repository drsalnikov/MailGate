using MassTransit;
using MailGate.Channels.Smtp.Services;
using MailGate.Infrastructure.Messaging;
using MailGate.Infrastructure.Persistence;
using MailGate.Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MailGate.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureBus = null,
        bool enableConsumerOutbox = false)
    {
        services.Configure<MassTransitOptions>(configuration.GetSection(MassTransitOptions.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=mailgate;Username=mailgate;Password=mailgate";

        services.AddDbContext<MailGateDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(MailGateDbContext).Assembly.FullName)));

        services.AddMemoryCache();
        services.AddDataProtection();
        services.AddScoped<SmtpConfigurationStore>();

        var rmqOptions = configuration.GetSection(MassTransitOptions.SectionName).Get<MassTransitOptions>()
            ?? new MassTransitOptions();

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            // Consumer-side Outbox: keeps messages published inside a consumer
            // in the DB until the consumer completes successfully (at-least-once delivery).
            // Only enabled for the Worker; the API publishes directly to the broker.
            if (enableConsumerOutbox)
            {
                bus.AddEntityFrameworkOutbox<MailGateDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });
            }

            configureBus?.Invoke(bus);

            bus.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rmqOptions.Host, (ushort)rmqOptions.Port, rmqOptions.VirtualHost, h =>
                {
                    h.Username(rmqOptions.Username);
                    h.Password(rmqOptions.Password);
                });

                cfg.UseMessageRetry(r => r.Exponential(3,
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromSeconds(10)));

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }

    /// <summary>
    /// Replaces the default <see cref="ISmtpSettingsProvider"/> (which reads only from appsettings)
    /// with <see cref="DbSmtpSettingsProvider"/>, enabling database-backed runtime overrides managed
    /// through the admin UI.
    /// </summary>
    public static IServiceCollection AddDbSmtpSettingsProvider(this IServiceCollection services)
    {
        services.RemoveAll<ISmtpSettingsProvider>();
        services.AddSingleton<DbSmtpSettingsProvider>();
        services.AddSingleton<ISmtpSettingsProvider>(sp => sp.GetRequiredService<DbSmtpSettingsProvider>());
        return services;
    }
}
