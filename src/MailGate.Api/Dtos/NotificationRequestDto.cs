using MailGate.Core.Enums;

namespace MailGate.Api.Dtos;

public sealed record NotificationRequestDto
{
    public ChannelType Channel { get; init; }
    public IReadOnlyList<string> Recipients { get; init; } = [];
    public string? Subject { get; init; }
    public string Body { get; init; } = string.Empty;
    public BodyFormat BodyFormat { get; init; } = BodyFormat.PlainText;
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public DateTimeOffset? ScheduledAt { get; init; }
    public IDictionary<string, string>? Metadata { get; init; }
}
