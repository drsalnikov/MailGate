using MailGate.Channels.Smtp.Extensions;
using MailGate.Core.Interfaces;
using MailGate.Infrastructure.Extensions;
using MailGate.Infrastructure.Persistence;
using MailGate.Worker.Consumers;
using MailGate.Worker.Routing;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, bus =>
{
    bus.AddConsumer<NotificationMessageConsumer>();
}, enableConsumerOutbox: true);

builder.Services.AddSmtpChannel(builder.Configuration);
builder.Services.AddDbSmtpSettingsProvider();

builder.Services.AddSingleton<IChannelRouter, ChannelRouter>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MailGateDbContext>();
    await db.Database.MigrateAsync();
}

await host.RunAsync();
