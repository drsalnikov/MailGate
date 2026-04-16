using MailGate.Core.Enums;

namespace MailGate.Core.Entities;

public sealed class NotificationLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    public string? IdempotencyKey { get; set; }
    public ChannelType Channel { get; set; }
    public string RecipientsJson { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public DeliveryStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public int AttemptCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; set; }
}
