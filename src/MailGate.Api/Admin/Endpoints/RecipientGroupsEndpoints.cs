using FluentValidation;
using MailGate.Contracts.Common;
using MailGate.Contracts.RecipientGroups;
using MailGate.Core.Entities;
using MailGate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Api.Admin.Endpoints;

public static class RecipientGroupsEndpoints
{
    public static IEndpointRouteBuilder MapRecipientGroupsAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/recipient-groups")
            .RequireAuthorization("MailGateAdmin")
            .WithTags("Admin / Recipient Groups");

        group.MapGet("/", ListAsync).WithName("ListRecipientGroups");
        group.MapGet("/{id:guid}", GetAsync).WithName("GetRecipientGroup");
        group.MapPost("/", CreateAsync).WithName("CreateRecipientGroup");
        group.MapPut("/{id:guid}", UpdateAsync).WithName("UpdateRecipientGroup");
        group.MapDelete("/{id:guid}", DeleteAsync).WithName("DeleteRecipientGroup");

        return app;
    }

    private static async Task<Ok<PagedResult<RecipientGroupDto>>> ListAsync(
        MailGateDbContext db,
        CancellationToken cancellationToken,
        string? search = null,
        int page = 1,
        int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 500) pageSize = 50;

        var query = db.RecipientGroups
            .AsNoTracking()
            .Include(x => x.Recipients)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, pattern) ||
                (x.Description != null && EF.Functions.ILike(x.Description, pattern)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(new PagedResult<RecipientGroupDto>(items, total, page, pageSize));
    }

    private static async Task<Results<Ok<RecipientGroupDto>, NotFound>> GetAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.RecipientGroups.AsNoTracking()
            .Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(entity));
    }

    private static async Task<Results<Created<RecipientGroupDto>, ValidationProblem, Conflict<string>>> CreateAsync(
        CreateRecipientGroupDto dto,
        IValidator<CreateRecipientGroupDto> validator,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var name = dto.Name.Trim();
        var exists = await db.RecipientGroups.AnyAsync(x => x.Name == name, cancellationToken);
        if (exists)
            return TypedResults.Conflict($"Recipient group with name '{name}' already exists.");

        var recipients = await LoadRecipientsAsync(db, dto.RecipientIds, cancellationToken);
        var entity = new RecipientGroup
        {
            Name = name,
            Description = NormalizeDescription(dto.Description),
            Recipients = recipients
        };

        db.RecipientGroups.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/api/admin/recipient-groups/{entity.Id}", ToDto(entity));
    }

    private static async Task<Results<Ok<RecipientGroupDto>, ValidationProblem, NotFound, Conflict<string>>> UpdateAsync(
        Guid id,
        UpdateRecipientGroupDto dto,
        IValidator<UpdateRecipientGroupDto> validator,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var entity = await db.RecipientGroups
            .Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        var name = dto.Name.Trim();
        if (!string.Equals(entity.Name, name, StringComparison.Ordinal))
        {
            var duplicate = await db.RecipientGroups.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
            if (duplicate)
                return TypedResults.Conflict($"Recipient group with name '{name}' already exists.");
        }

        entity.Name = name;
        entity.Description = NormalizeDescription(dto.Description);
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.Recipients.Clear();

        var recipients = await LoadRecipientsAsync(db, dto.RecipientIds, cancellationToken);
        foreach (var recipient in recipients)
            entity.Recipients.Add(recipient);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ToDto(entity));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.RecipientGroups.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        db.RecipientGroups.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    private static RecipientGroupDto ToDto(RecipientGroup entity) => new(
        entity.Id,
        entity.Name,
        entity.Description,
        entity.Recipients
            .OrderBy(x => x.FullName)
            .Select(x => new GroupRecipientDto(x.Id, x.FullName, x.Email))
            .ToList(),
        entity.CreatedAt,
        entity.UpdatedAt);

    private static async Task<List<Recipient>> LoadRecipientsAsync(
        MailGateDbContext db,
        IReadOnlyList<Guid> recipientIds,
        CancellationToken cancellationToken)
    {
        if (recipientIds.Count == 0)
            return [];

        var ids = recipientIds.Distinct().ToList();
        return await db.Recipients
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    private static string? NormalizeDescription(string? description)
        => string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
