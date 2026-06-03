namespace MailGate.Contracts.Smtp;

public enum SmtpConfigurationSource
{
    AppSettings,
    Database
}

public sealed record SmtpConfigurationDto(
    string Host,
    int Port,
    bool UseSsl,
    bool UseTls,
    string FromAddress,
    string FromName,
    string? Username,
    bool HasPassword,
    SmtpOAuth2Dto? OAuth2,
    SmtpConfigurationSource Source,
    DateTimeOffset? UpdatedAt);

public sealed record SmtpOAuth2Dto(
    string TenantId,
    string ClientId,
    bool HasClientSecret,
    string SmtpUserPrincipalName);

public sealed class UpdateSmtpConfigurationDto
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; }
    public bool UseTls { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public UpdateSmtpOAuth2Dto? OAuth2 { get; set; }
}

public sealed class UpdateSmtpOAuth2Dto
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
    public string SmtpUserPrincipalName { get; set; } = string.Empty;
}

public sealed record SmtpTestResultDto(
    bool Success,
    string? ErrorMessage,
    long DurationMs);
