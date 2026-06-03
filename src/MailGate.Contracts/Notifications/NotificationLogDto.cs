using MailGate.Core.Enums;

namespace MailGate.Contracts.Notifications;

public sealed record NotificationLogDto(
    Guid Id,
    Guid MessageId,
    string? IdempotencyKey,
    ChannelType Channel,
    IReadOnlyList<string> Recipients,
    string? Subject,
    DeliveryStatus Status,
    string? ErrorMessage,
    int AttemptCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ProcessedAt,
    Guid? RetryOfMessageId);

public sealed record NotificationLogDetailsDto(
    Guid Id,
    Guid MessageId,
    string? IdempotencyKey,
    ChannelType Channel,
    IReadOnlyList<string> Recipients,
    string? Subject,
    string? Body,
    BodyFormat BodyFormat,
    NotificationPriority Priority,
    DeliveryStatus Status,
    string? ErrorMessage,
    int AttemptCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ProcessedAt,
    Guid? RetryOfMessageId);

public sealed class SendNotificationFromAdminDto
{
    public ChannelType Channel { get; set; } = ChannelType.Smtp;
    public IReadOnlyList<Guid> RecipientIds { get; set; } = [];
    public IReadOnlyList<Guid> RecipientGroupIds { get; set; } = [];
    public IReadOnlyList<string> AdHocEmails { get; set; } = [];
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public BodyFormat BodyFormat { get; set; } = BodyFormat.PlainText;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
}

public sealed record RetryNotificationResponseDto(Guid NewMessageId, Guid OriginalMessageId);

public sealed record NotificationStatsDto(
    int SentLast24h,
    int FailedLast24h,
    int SkippedLast24h,
    int TotalRecipients);
