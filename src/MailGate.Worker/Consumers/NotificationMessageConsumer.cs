using MassTransit;
using MailGate.Core.Entities;
using MailGate.Core.Enums;
using MailGate.Core.Interfaces;
using MailGate.Core.Models;
using MailGate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MailGate.Worker.Consumers;

public sealed class NotificationMessageConsumer(
    IChannelRouter router,
    MailGateDbContext dbContext,
    ILogger<NotificationMessageConsumer> logger) : IConsumer<NotificationMessage>
{
    public async Task Consume(ConsumeContext<NotificationMessage> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Processing notification MessageId={MessageId}, Channel={Channel}, Recipients={Count}",
            message.MessageId, message.Channel, message.Recipients.Count);

        if (!router.IsSupported(message.Channel))
        {
            logger.LogWarning(
                "Channel {Channel} is not supported. Skipping MessageId={MessageId}",
                message.Channel, message.MessageId);

            await UpsertLogAsync(message, DeliveryStatus.Skipped,
                $"Channel '{message.Channel}' is not registered.", context.CancellationToken);
            return;
        }

        var channel = router.Resolve(message.Channel);
        var result = await channel.SendAsync(message, context.CancellationToken);

        await UpsertLogAsync(message, result.Status, result.ErrorMessage, context.CancellationToken);

        if (result.Status == DeliveryStatus.Failed)
        {
            logger.LogError(
                "Delivery failed for MessageId={MessageId}: {Error}",
                message.MessageId, result.ErrorMessage);

            // Throwing causes MassTransit to retry with exponential backoff.
            throw new InvalidOperationException(
                $"Delivery failed for MessageId={message.MessageId}: {result.ErrorMessage}");
        }

        logger.LogInformation(
            "Notification delivered. MessageId={MessageId}, Status={Status}",
            message.MessageId, result.Status);
    }

    private async Task UpsertLogAsync(
        NotificationMessage message,
        DeliveryStatus status,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        // Find existing log by MessageId (covers retries with the same message).
        var existing = await dbContext.NotificationLogs
            .FirstOrDefaultAsync(l => l.MessageId == message.MessageId, cancellationToken);

        if (existing is not null)
        {
            existing.Status = status;
            existing.ErrorMessage = errorMessage;
            existing.AttemptCount += 1;
            existing.ProcessedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            dbContext.NotificationLogs.Add(new NotificationLog
            {
                MessageId = message.MessageId,
                IdempotencyKey = message.IdempotencyKey,
                Channel = message.Channel,
                RecipientsJson = JsonSerializer.Serialize(message.Recipients),
                Subject = message.Subject,
                Status = status,
                ErrorMessage = errorMessage,
                AttemptCount = 1,
                ProcessedAt = DateTimeOffset.UtcNow
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
