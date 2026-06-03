namespace MailGate.Core.Entities;

public sealed class Recipient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public ICollection<NotificationSystem> Systems { get; set; } = [];
    public ICollection<RecipientGroup> Groups { get; set; } = [];
}
