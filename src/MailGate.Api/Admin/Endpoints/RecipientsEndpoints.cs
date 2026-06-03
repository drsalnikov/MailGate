using FluentValidation;
using MailGate.Contracts.Common;
using MailGate.Contracts.Recipients;
using MailGate.Core.Entities;
using MailGate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Api.Admin.Endpoints;

public static class RecipientsEndpoints
{
    public static IEndpointRouteBuilder MapRecipientsAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/recipients")
            .RequireAuthorization("MailGateAdmin")
            .WithTags("Admin / Recipients");

        group.MapGet("/", ListAsync)
            .WithName("ListRecipients");

        group.MapGet("/{id:guid}", GetAsync)
            .WithName("GetRecipient");

        group.MapPost("/", CreateAsync)
            .WithName("CreateRecipient");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateRecipient");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteRecipient");

        return app;
    }

    private static async Task<Ok<PagedResult<RecipientDto>>> ListAsync(
        MailGateDbContext db,
        CancellationToken cancellationToken,
        string? search = null,
        int page = 1,
        int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 500) pageSize = 50;

        var query = db.Recipients
            .AsNoTracking()
            .Include(x => x.Systems)
            .Include(x => x.Groups)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.FullName, pattern) ||
                EF.Functions.ILike(x.Email, pattern));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(new PagedResult<RecipientDto>(items, total, page, pageSize));
    }

    private static async Task<Results<Ok<RecipientDto>, NotFound>> GetAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.Recipients.AsNoTracking()
            .Include(x => x.Systems)
            .Include(x => x.Groups)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ToDto(entity));
    }

    private static async Task<Results<Created<RecipientDto>, ValidationProblem, Conflict<string>>> CreateAsync(
        CreateRecipientDto dto,
        IValidator<CreateRecipientDto> validator,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var email = dto.Email.Trim().ToLowerInvariant();

        var exists = await db.Recipients.AnyAsync(x => x.Email == email, cancellationToken);
        if (exists)
            return TypedResults.Conflict($"Recipient with email '{email}' already exists.");

        var systems = await LoadSystemsAsync(db, dto.SystemIds, cancellationToken);

        var entity = new Recipient
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Systems = systems
        };

        db.Recipients.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        var result = ToDto(entity);
        return TypedResults.Created($"/api/admin/recipients/{entity.Id}", result);
    }

    private static async Task<Results<Ok<RecipientDto>, ValidationProblem, NotFound, Conflict<string>>> UpdateAsync(
        Guid id,
        UpdateRecipientDto dto,
        IValidator<UpdateRecipientDto> validator,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var entity = await db.Recipients
            .Include(x => x.Systems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        var email = dto.Email.Trim().ToLowerInvariant();

        if (!string.Equals(entity.Email, email, StringComparison.Ordinal))
        {
            var duplicate = await db.Recipients
                .AnyAsync(x => x.Id != id && x.Email == email, cancellationToken);
            if (duplicate)
                return TypedResults.Conflict($"Recipient with email '{email}' already exists.");
        }

        entity.FullName = dto.FullName.Trim();
        entity.Email = email;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.Systems.Clear();

        var systems = await LoadSystemsAsync(db, dto.SystemIds, cancellationToken);
        foreach (var system in systems)
            entity.Systems.Add(system);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ToDto(entity));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.Recipients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        db.Recipients.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    private static RecipientDto ToDto(Recipient entity) => new(
        entity.Id,
        entity.FullName,
        entity.Email,
        entity.Systems
            .OrderBy(x => x.Name)
            .Select(x => new RecipientSystemDto(x.Id, x.Code, x.Name))
            .ToList(),
        entity.Groups
            .OrderBy(x => x.Name)
            .Select(x => new RecipientGroupSummaryDto(x.Id, x.Name))
            .ToList(),
        entity.CreatedAt,
        entity.UpdatedAt);

    private static async Task<List<NotificationSystem>> LoadSystemsAsync(
        MailGateDbContext db,
        IReadOnlyList<Guid> systemIds,
        CancellationToken cancellationToken)
    {
        if (systemIds.Count == 0)
            return [];

        var ids = systemIds.Distinct().ToList();
        return await db.Systems
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
