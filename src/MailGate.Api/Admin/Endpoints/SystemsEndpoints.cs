using FluentValidation;
using MailGate.Contracts.Common;
using MailGate.Contracts.Systems;
using MailGate.Core.Entities;
using MailGate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Api.Admin.Endpoints;

public static class SystemsEndpoints
{
    public static IEndpointRouteBuilder MapSystemsAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/systems")
            .RequireAuthorization("MailGateAdmin")
            .WithTags("Admin / Systems");

        group.MapGet("/", ListAsync).WithName("ListSystems");
        group.MapGet("/{id:guid}", GetAsync).WithName("GetSystem");
        group.MapPost("/", CreateAsync).WithName("CreateSystem");
        group.MapPut("/{id:guid}", UpdateAsync).WithName("UpdateSystem");
        group.MapDelete("/{id:guid}", DeleteAsync).WithName("DeleteSystem");

        return app;
    }

    private static async Task<Ok<PagedResult<SystemDto>>> ListAsync(
        MailGateDbContext db,
        CancellationToken cancellationToken,
        string? search = null,
        int page = 1,
        int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 500) pageSize = 50;

        var query = db.Systems.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.Code, pattern) ||
                EF.Functions.ILike(x.Name, pattern) ||
                (x.Description != null && EF.Functions.ILike(x.Description, pattern)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SystemDto(
                x.Id,
                x.Code,
                x.Name,
                x.Description,
                x.Recipients.Count,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(new PagedResult<SystemDto>(items, total, page, pageSize));
    }

    private static async Task<Results<Ok<SystemDto>, NotFound>> GetAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.Systems.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new SystemDto(
                x.Id,
                x.Code,
                x.Name,
                x.Description,
                x.Recipients.Count,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return entity is null ? TypedResults.NotFound() : TypedResults.Ok(entity);
    }

    private static async Task<Results<Created<SystemDto>, ValidationProblem, Conflict<string>>> CreateAsync(
        CreateSystemDto dto,
        IValidator<CreateSystemDto> validator,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var code = NormalizeCode(dto.Code);
        var exists = await db.Systems.AnyAsync(x => x.Code == code, cancellationToken);
        if (exists)
            return TypedResults.Conflict($"System with code '{code}' already exists.");

        var entity = new NotificationSystem
        {
            Code = code,
            Name = dto.Name.Trim(),
            Description = NormalizeDescription(dto.Description)
        };

        db.Systems.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        var result = ToDto(entity, subscriberCount: 0);
        return TypedResults.Created($"/api/admin/systems/{entity.Id}", result);
    }

    private static async Task<Results<Ok<SystemDto>, ValidationProblem, NotFound, Conflict<string>>> UpdateAsync(
        Guid id,
        UpdateSystemDto dto,
        IValidator<UpdateSystemDto> validator,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var entity = await db.Systems
            .Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        var code = NormalizeCode(dto.Code);
        if (!string.Equals(entity.Code, code, StringComparison.Ordinal))
        {
            var duplicate = await db.Systems.AnyAsync(x => x.Id != id && x.Code == code, cancellationToken);
            if (duplicate)
                return TypedResults.Conflict($"System with code '{code}' already exists.");
        }

        entity.Code = code;
        entity.Name = dto.Name.Trim();
        entity.Description = NormalizeDescription(dto.Description);
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ToDto(entity, entity.Recipients.Count));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(
        Guid id,
        MailGateDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.Systems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        db.Systems.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    private static SystemDto ToDto(NotificationSystem entity, int subscriberCount) => new(
        entity.Id,
        entity.Code,
        entity.Name,
        entity.Description,
        subscriberCount,
        entity.CreatedAt,
        entity.UpdatedAt);

    private static string NormalizeCode(string code) => code.Trim().ToLowerInvariant();

    private static string? NormalizeDescription(string? description)
        => string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
