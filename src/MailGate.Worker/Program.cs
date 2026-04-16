using MailGate.Channels.Smtp.Extensions;
using MailGate.Core.Interfaces;
using MailGate.Infrastructure.Extensions;
using MailGate.Worker.Consumers;
using MailGate.Worker.Routing;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, bus =>
{
    bus.AddConsumer<NotificationMessageConsumer>();
}, enableConsumerOutbox: true);

builder.Services.AddSmtpChannel(builder.Configuration);

builder.Services.AddSingleton<IChannelRouter, ChannelRouter>();

var host = builder.Build();
host.Run();
