using MailGate.Contracts.Templates;
using MailGate.Core.Enums;
using MailGate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Api.Templates;

public sealed record ComposedNotification(string? Subject, string Body, BodyFormat BodyFormat);
public sealed class NotificationCompositionException(string message) : Exception(message);

public static class NotificationComposer
{
    public static async Task<ComposedNotification> ComposeAsync(MailGateDbContext db, PreviewNotificationDto dto, CancellationToken ct)
    {
        if (dto.Body is null || dto.Body.Length > 1024 * 1024) throw new NotificationCompositionException("Текст: до 1 МБ.");
        if (dto.Subject?.Length > 500 || dto.Subject?.Contains('\r') == true || dto.Subject?.Contains('\n') == true)
            throw new NotificationCompositionException("Тема: до 500 символов, без перевода строки.");
        if (!Enum.IsDefined(dto.BodyFormat)) throw new NotificationCompositionException("Неизвестный формат сообщения.");
        if (dto.TemplateId is not { } id)
        {
            if (dto.TemplateDefinition is not null) throw new NotificationCompositionException("Выберите исходный шаблон.");
            return new(dto.Subject, dto.Body, dto.BodyFormat);
        }
        var template = await db.EmailTemplates.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotificationCompositionException("Шаблон удалён или не найден. Выберите другой шаблон.");
        if (dto.TemplateRevision != template.Revision)
            throw new NotificationCompositionException("Шаблон изменился. Выберите его заново, чтобы обновить предпросмотр.");
        var definition = dto.TemplateDefinition ?? Admin.Endpoints.EmailTemplatesEndpoints.Deserialize(template);
        var errors = EmailTemplateRenderer.Validate(definition);
        if (errors.Count != 0)
            throw new NotificationCompositionException(string.Join(" ", errors.Values.SelectMany(messages => messages)));
        var result = EmailTemplateRenderer.Render(definition, dto.Body, dto.Subject);
        return new(result.Subject, result.Html, BodyFormat.Html);
    }
    public static EmailPreviewDto Preview(ComposedNotification composed) => new(composed.Subject ?? "",
        composed.BodyFormat == BodyFormat.PlainText
            ? $"<!doctype html><html><head><meta charset=\"utf-8\"></head><body><pre style=\"white-space:pre-wrap;overflow-wrap:anywhere;font:16px/1.6 Arial\">{EmailTemplateRenderer.Encode(composed.Body)}</pre></body></html>"
            : composed.Body);
}
