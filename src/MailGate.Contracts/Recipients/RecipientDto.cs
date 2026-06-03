namespace MailGate.Contracts.Recipients;

public sealed record RecipientDto(
    Guid Id,
    string FullName,
    string Email,
    IReadOnlyList<RecipientSystemDto> Systems,
    IReadOnlyList<RecipientGroupSummaryDto> Groups,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed record RecipientSystemDto(
    Guid Id,
    string Code,
    string Name);

public sealed record RecipientGroupSummaryDto(
    Guid Id,
    string Name);

public sealed class CreateRecipientDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<Guid> SystemIds { get; set; } = [];
}

public sealed class UpdateRecipientDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<Guid> SystemIds { get; set; } = [];
}
