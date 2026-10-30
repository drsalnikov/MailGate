namespace MailGate.Contracts.Templates;

public sealed class EmailTemplateDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Preheader { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#1677ff";
    public string BackgroundColor { get; set; } = "#f3f5f7";
    public string Footer { get; set; } = string.Empty;
    public List<EmailTemplateBlock> Blocks { get; set; } = [];
}

public sealed class EmailTemplateBlock
{
    public string Type { get; set; } = "text";
    public string Text { get; set; } = string.Empty;
    public string SecondaryText { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public sealed record EmailTemplateDto(Guid Id, EmailTemplateDefinition Definition, Guid Revision,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
public sealed record EmailTemplateSummaryDto(Guid Id, string Name, string Subject, Guid Revision, DateTimeOffset UpdatedAt);
public sealed class SaveEmailTemplateDto
{
    public EmailTemplateDefinition Definition { get; set; } = new();
    public Guid? Revision { get; set; }
}
public sealed class PreviewEmailTemplateDto
{
    public EmailTemplateDefinition Definition { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string? Subject { get; set; }
}
public sealed class PreviewNotificationDto
{
    public EmailTemplateDefinition? TemplateDefinition { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? TemplateRevision { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public MailGate.Core.Enums.BodyFormat BodyFormat { get; set; }
}
public sealed record EmailPreviewDto(string Subject, string Html);
