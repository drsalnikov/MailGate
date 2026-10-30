namespace MailGate.Channels.Smtp.Options;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public int SettingsCacheSeconds { get; set; } = 30;

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = false;
    public bool UseTls { get; set; } = true;
    public bool CheckCertificateRevocation { get; set; } = true;
    public string FromAddress { get; set; } = "noreply@example.com";
    public string FromName { get; set; } = "MailGate";

    /// <summary>Basic auth credentials (not used when OAuth2 is configured).</summary>
    public string? Username { get; set; }
    public string? Password { get; set; }

    public OAuth2Options? OAuth2 { get; set; }
}

public sealed class OAuth2Options
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SmtpUserPrincipalName { get; set; } = string.Empty;
}
