namespace MailGate.Contracts.RecipientGroups;

public sealed record RecipientGroupDto(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<GroupRecipientDto> Recipients,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt)
{
    public int RecipientCount => Recipients.Count;
}

public sealed record GroupRecipientDto(
    Guid Id,
    string FullName,
    string Email);

public sealed class CreateRecipientGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyList<Guid> RecipientIds { get; set; } = [];
}

public sealed class UpdateRecipientGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyList<Guid> RecipientIds { get; set; } = [];
}
