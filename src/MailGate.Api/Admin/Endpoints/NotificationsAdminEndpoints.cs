using System.Text.Json;
using FluentValidation;
using MailGate.Contracts.Common;
using MailGate.Contracts.Notifications;
using MailGate.Core.Enums;
using MailGate.Core.Models;
using MailGate.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Api.Admin.Endpoints;

public static class NotificationsAdminEndpoints
{
    private static readonly JsonSerializerOptions PayloadOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static IEndpointRouteBuilder MapNotificationsAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/notifications")
            .RequireAuthorization("MailGateAdmin")
            .WithTags("Admin / Notifications");

        group.MapGet("/", ListAsync).WithName("ListAdminNotifications");
        group.MapGet("/stats", StatsAsync).WithName("AdminNotificationStats");
        group.MapGet("/{id:guid}", GetAsync).WithName("GetAdminNotification");
        group.MapPost("/", SendAsync).WithName("AdminSendNotification");
        group.MapPost("/{id:guid}/retry", RetryAsync).WithName("AdminRetryNotification");

        return app;
    }

    private static async Task<Ok<PagedResult<NotificationLogDto>>> ListAsync(
        MailGateDbContext db,
        CancellationToken cancellationToken,
        DeliveryStatus? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 500) pageSize = 50;

        var query = db.NotificationLogs.AsNoTracking();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(x =>
                (x.Subject != null && EF.Functions.ILike(x.Subject, pattern)) ||
                EF.Functions.ILike(x.RecipientsJson, pattern) ||
                (x.IdempotencyKey != null && EF.Functions.ILike(x.IdempotencyKey, pattern)));
        }

        var total = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x => new NotificationLogDto(
            x.Id,
            x.MessageId,
            x.IdempotencyKey,
            x.Channel,
            DeserializeRecipients(x.RecipientsJson),
            x.Subject,
            x.Status,
            x.ErrorMessage,
            x.AttemptCount,
            x.CreatedAt,
            x.ProcessedAt,
            x.RetryOfMessageId)).ToList();

        return TypedResults.Ok(new PagedResult<NotificationLogDto>(items, total, page, pageSize));
    }

    private static async Task<Ok<NotificationStatsDto>> StatsAsync(
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var since = DateTimeOffset.UtcNow.AddHours(-24);

        var recent = await db.NotificationLogs.AsNoTracking()
            .Where(x => x.CreatedAt >= since)
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalRecipients = await db.Recipients.CountAsync(cancellationToken);

        return TypedResults.Ok(new NotificationStatsDto(
            SentLast24h: recent.FirstOrDefault(r => r.Status == DeliveryStatus.Sent)?.Count ?? 0,
            FailedLast24h: recent.FirstOrDefault(r => r.Status == DeliveryStatus.Failed)?.Count ?? 0,
            SkippedLast24h: recent.FirstOrDefault(r => r.Status == DeliveryStatus.Skipped)?.Count ?? 0,
            TotalRecipients: totalRecipients));
    }

    private static async Task<Results<Ok<NotificationLogDetailsDto>, NotFound>> GetAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var log = await db.NotificationLogs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (log is null)
            return TypedResults.NotFound();

        var payload = DeserializePayload(log.PayloadJson);

        return TypedResults.Ok(new NotificationLogDetailsDto(
            log.Id,
            log.MessageId,
            log.IdempotencyKey,
            log.Channel,
            payload?.Recipients.ToList() ?? DeserializeRecipients(log.RecipientsJson),
            log.Subject,
            payload?.Body,
            payload?.BodyFormat ?? BodyFormat.PlainText,
            payload?.Priority ?? NotificationPriority.Normal,
            log.Status,
            log.ErrorMessage,
            log.AttemptCount,
            log.CreatedAt,
            log.ProcessedAt,
            log.RetryOfMessageId));
    }

    private static async Task<Results<Accepted<AcceptedNotificationDto>, ValidationProblem>> SendAsync(
        SendNotificationFromAdminDto dto,
        IValidator<SendNotificationFromAdminDto> validator,
        MailGateDbContext db,
        IPublishEndpoint publisher,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var emailsFromBook = dto.RecipientIds.Count == 0
            ? []
            : await db.Recipients.AsNoTracking()
                .Where(x => dto.RecipientIds.Contains(x.Id))
                .Select(x => x.Email)
                .ToListAsync(cancellationToken);

        var emailsFromGroups = dto.RecipientGroupIds.Count == 0
            ? []
            : await db.RecipientGroups.AsNoTracking()
                .Where(x => dto.RecipientGroupIds.Contains(x.Id))
                .SelectMany(x => x.Recipients.Select(r => r.Email))
                .ToListAsync(cancellationToken);

        var allRecipients = emailsFromBook
            .Concat(emailsFromGroups)
            .Concat(dto.AdHocEmails.Select(e => e.Trim().ToLowerInvariant()))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (allRecipients.Count == 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["Recipients"] = ["Resolved recipients list is empty."]
            });
        }

        var message = new NotificationMessage
        {
            Channel = dto.Channel,
            Recipients = allRecipients,
            Subject = dto.Subject,
            Body = dto.Body,
            BodyFormat = dto.BodyFormat,
            Priority = dto.Priority,
            TraceId = httpContext.TraceIdentifier
        };

        await publisher.Publish(message, cancellationToken);

        return TypedResults.Accepted(
            $"/api/admin/notifications/{message.MessageId}",
            new AcceptedNotificationDto(message.MessageId));
    }

    private static async Task<Results<Accepted<RetryNotificationResponseDto>, NotFound, ProblemHttpResult>> RetryAsync(
        Guid id,
        MailGateDbContext db,
        IPublishEndpoint publisher,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var log = await db.NotificationLogs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (log is null)
            return TypedResults.NotFound();

        var payload = DeserializePayload(log.PayloadJson);
        if (payload is null)
        {
            return TypedResults.Problem(
                detail: "Original payload is not available for retry. Only notifications processed after the admin UI upgrade can be retried.",
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        var retry = new NotificationMessage
        {
            Channel = payload.Channel,
            Recipients = payload.Recipients,
            Subject = payload.Subject,
            Body = payload.Body,
            BodyFormat = payload.BodyFormat,
            Priority = payload.Priority,
            Metadata = payload.Metadata,
            TraceId = httpContext.TraceIdentifier
        };

        await publisher.Publish(retry, ctx =>
        {
            ctx.Headers.Set("MailGate-RetryOf-MessageId", log.MessageId);
        }, cancellationToken);

        return TypedResults.Accepted(
            $"/api/admin/notifications/{retry.MessageId}",
            new RetryNotificationResponseDto(retry.MessageId, log.MessageId));
    }

    private static List<string> DeserializeRecipients(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static NotificationMessage? DeserializePayload(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<NotificationMessage>(json, PayloadOptions);
        }
        catch
        {
            return null;
        }
    }
}

public sealed record AcceptedNotificationDto(Guid MessageId);
