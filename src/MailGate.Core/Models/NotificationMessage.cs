using MailGate.Core.Enums;

namespace MailGate.Core.Models;

/// <summary>
/// Message published to RabbitMQ and consumed by the worker.
/// </summary>
public sealed record NotificationMessage
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public string? IdempotencyKey { get; init; }
    public ChannelType Channel { get; init; }
    public IReadOnlyList<string> Recipients { get; init; } = [];
    public string? Subject { get; init; }
    public string Body { get; init; } = string.Empty;
    public BodyFormat BodyFormat { get; init; } = BodyFormat.PlainText;
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public DateTimeOffset? ScheduledAt { get; init; }
    public IDictionary<string, string>? Metadata { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? TraceId { get; init; }
}
