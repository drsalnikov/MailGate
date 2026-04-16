namespace MailGate.Api.Dtos;

public sealed record NotificationResponseDto(Guid MessageId, string Status, DateTimeOffset AcceptedAt);
