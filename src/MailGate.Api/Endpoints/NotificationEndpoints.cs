using FluentValidation;
using MassTransit;
using MailGate.Api.Dtos;
using MailGate.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MailGate.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications");

        group.MapPost("/", SendNotificationAsync)
            .WithName("SendNotification")
            .WithSummary("Submit a notification request for delivery")
            .Produces<NotificationResponseDto>(StatusCodes.Status202Accepted)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<IResult> SendNotificationAsync(
        NotificationRequestDto dto,
        IValidator<NotificationRequestDto> validator,
        IPublishEndpoint publisher,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors);
        }

        var idempotencyKey = httpContext.Request.Headers["X-Idempotency-Key"].FirstOrDefault();
        var traceId = httpContext.TraceIdentifier;

        var message = new NotificationMessage
        {
            IdempotencyKey = idempotencyKey,
            Channel = dto.Channel,
            Recipients = dto.Recipients,
            Subject = dto.Subject,
            Body = dto.Body,
            BodyFormat = dto.BodyFormat,
            Priority = dto.Priority,
            ScheduledAt = dto.ScheduledAt,
            Metadata = dto.Metadata,
            TraceId = traceId
        };

        await publisher.Publish(message, cancellationToken);

        return Results.Accepted(
            value: new NotificationResponseDto(message.MessageId, "Accepted", DateTimeOffset.UtcNow));
    }
}
