namespace MailGate.Infrastructure.Messaging;

public sealed class MassTransitOptions
{
    public const string SectionName = "MassTransit";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string NotificationsQueue { get; set; } = "mailgate-notifications";
}
