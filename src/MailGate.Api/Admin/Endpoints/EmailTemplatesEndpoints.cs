using System.Text.Json;
using MailGate.Api.Templates;
using MailGate.Contracts.Common;
using MailGate.Contracts.Templates;
using MailGate.Core.Entities;
using MailGate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Api.Admin.Endpoints;

public static class EmailTemplatesEndpoints
{
    public static IEndpointRouteBuilder MapEmailTemplatesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/email-templates").RequireAuthorization("MailGateAdmin").WithTags("Admin / Email templates");
        group.MapGet("/", ListAsync);
        group.MapGet("/{id:guid}", GetAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:guid}", UpdateAsync);
        group.MapDelete("/{id:guid}", DeleteAsync);
        group.MapPost("/preview", Preview);
        return app;
    }

    private static async Task<IResult> ListAsync(MailGateDbContext db, CancellationToken ct,
        string? search = null, int page = 1, int pageSize = 50)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 500);
        var query = db.EmailTemplates.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(t => EF.Functions.ILike(t.Name, $"%{search.Trim()}%"));
        var count = await query.CountAsync(ct);
        var rows = await query.OrderByDescending(t => t.UpdatedAt).ThenBy(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Results.Ok(new PagedResult<EmailTemplateSummaryDto>(rows.Select(t => new EmailTemplateSummaryDto(
            t.Id, t.Name, Deserialize(t).Subject, t.Revision, t.UpdatedAt)).ToList(), count, page, pageSize));
    }
    private static async Task<IResult> GetAsync(Guid id, MailGateDbContext db, CancellationToken ct)
    {
        var entity = await db.EmailTemplates.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, ct);
        return entity is null ? Results.NotFound() : Results.Ok(ToDto(entity));
    }
    private static async Task<IResult> CreateAsync(SaveEmailTemplateDto dto, MailGateDbContext db, CancellationToken ct)
    {
        var errors = EmailTemplateRenderer.Validate(dto.Definition);
        if (errors.Count != 0) return Results.ValidationProblem(errors);
        var entity = new EmailTemplate { Name = dto.Definition.Name.Trim(), DefinitionJson = JsonSerializer.Serialize(dto.Definition) };
        db.EmailTemplates.Add(entity); await db.SaveChangesAsync(ct);
        return Results.Created($"/api/admin/email-templates/{entity.Id}", ToDto(entity));
    }
    private static async Task<IResult> UpdateAsync(Guid id, SaveEmailTemplateDto dto, MailGateDbContext db, CancellationToken ct)
    {
        var errors = EmailTemplateRenderer.Validate(dto.Definition);
        if (errors.Count != 0) return Results.ValidationProblem(errors);
        var entity = await db.EmailTemplates.SingleOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return Results.NotFound();
        if (dto.Revision != entity.Revision) return Results.Problem(statusCode:409, detail:"Шаблон изменён другим пользователем. Обновите страницу.");
        entity.Name = dto.Definition.Name.Trim(); entity.DefinitionJson = JsonSerializer.Serialize(dto.Definition);
        entity.Revision = Guid.NewGuid(); entity.UpdatedAt = DateTimeOffset.UtcNow;
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { return Results.Problem(statusCode:409, detail:"Шаблон изменён или удалён другим пользователем."); }
        return Results.Ok(ToDto(entity));
    }
    private static async Task<IResult> DeleteAsync(Guid id, MailGateDbContext db, CancellationToken ct)
    {
        var entity = await db.EmailTemplates.SingleOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return Results.NotFound();
        db.EmailTemplates.Remove(entity);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { return Results.Problem(statusCode:409, detail:"Шаблон изменён или удалён другим пользователем."); }
        return Results.NoContent();
    }
    private static IResult Preview(PreviewEmailTemplateDto dto)
    {
        var errors = EmailTemplateRenderer.Validate(dto.Definition);
        if (dto.Body is null || dto.Body.Length > 1024 * 1024) errors["Body"] = ["Текст: до 1 МБ."];
        if (dto.Subject?.Length > 500 || dto.Subject?.Contains('\r') == true || dto.Subject?.Contains('\n') == true) errors["Subject"] = ["Некорректная тема."];
        return errors.Count != 0 ? Results.ValidationProblem(errors) : Results.Ok(EmailTemplateRenderer.Render(dto.Definition, dto.Body!, dto.Subject));
    }
    public static EmailTemplateDefinition Deserialize(EmailTemplate entity) => JsonSerializer.Deserialize<EmailTemplateDefinition>(entity.DefinitionJson)
        ?? throw new InvalidOperationException("Шаблон повреждён.");
    private static EmailTemplateDto ToDto(EmailTemplate entity) => new(entity.Id, Deserialize(entity), entity.Revision, entity.CreatedAt, entity.UpdatedAt);
}
