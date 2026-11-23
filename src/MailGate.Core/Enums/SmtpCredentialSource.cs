namespace MailGate.Core.Enums;

public enum SmtpCredentialSource
{
    Manual = 0,
    Sidecar = 1,
    // Read existing installations without decrypting or deleting them on upgrade.
    Legacy = 2
}
