namespace MailGate.Contracts.Systems;

public sealed record SystemDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    int SubscriberCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed class CreateSystemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateSystemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
