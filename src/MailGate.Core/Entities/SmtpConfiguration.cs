namespace MailGate.Core.Entities;

/// <summary>
/// Singleton override of SMTP settings stored in the database.
/// Only one active row is expected (enforced at application level).
/// Secrets (Password, OAuth2 ClientSecret) are encrypted via ASP.NET Core Data Protection.
/// </summary>
public sealed class SmtpConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; }
    public bool UseTls { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "MailGate";
    public string? Username { get; set; }
    public string? EncryptedPassword { get; set; }

    public string? OAuth2TenantId { get; set; }
    public string? OAuth2ClientId { get; set; }
    public string? OAuth2EncryptedClientSecret { get; set; }
    public string? OAuth2SmtpUserPrincipalName { get; set; }

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
