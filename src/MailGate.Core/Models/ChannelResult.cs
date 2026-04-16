using MailGate.Core.Enums;

namespace MailGate.Core.Models;

public sealed record ChannelResult
{
    public DeliveryStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTimeOffset SentAt { get; init; } = DateTimeOffset.UtcNow;

    public static ChannelResult Success() =>
        new() { Status = DeliveryStatus.Sent };

    public static ChannelResult Failure(string error) =>
        new() { Status = DeliveryStatus.Failed, ErrorMessage = error };

    public static ChannelResult Skip(string reason) =>
        new() { Status = DeliveryStatus.Skipped, ErrorMessage = reason };
}
