using System.Text.Json;
using MailGate.Channels.Smtp.Options;
using MailGate.Channels.Smtp.Services;
using MailGate.Core.Enums;
using Microsoft.Extensions.Options;

var checks = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); checks++; }
var directory = Path.Combine(Path.GetTempPath(), "mailgate-credentials-" + Guid.NewGuid());
Directory.CreateDirectory(directory);
try
{
    var path = Path.Combine(directory, "account.json");
    var config = new SmtpOptions { CredentialsFilePath = path };
    var resolver = new SmtpCredentialResolver(new Monitor(config));
    SmtpOptions Sidecar() => new() { CredentialSource = SmtpCredentialSource.Sidecar, UseTls = true, Username = "old", Password = "must-not-fallback" };
    async Task Reject(SmtpOptions options, string name)
    {
        try { await resolver.ApplyAsync(options); throw new Exception("Accepted: " + name); }
        catch (InvalidOperationException e)
        {
            Check(!e.Message.Contains("private-test-value") && e.InnerException is null, name + ": sanitized error");
            Check(options.Username is null && options.Password is null, name + ": no fallback");
        }
    }
    var manual = new SmtpOptions { CredentialSource = SmtpCredentialSource.Manual, Username = "old", Password = "old", OAuth2 = new() };
    await resolver.ApplyAsync(manual);
    Check(manual.Username is null && manual.Password is null && manual.OAuth2 is null, "Manual clears all auth without reading a file");
    var legacy = new SmtpOptions { CredentialSource = SmtpCredentialSource.Legacy, Username = "old", Password = "legacy" };
    await resolver.ApplyAsync(legacy);
    Check(legacy.Password == "legacy", "Legacy remains compatible");
    await Reject(Sidecar(), "Missing file");
    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(new { username = "first", password = "private-test-value" }));
    var account = Sidecar(); await resolver.ApplyAsync(account);
    Check(account.Username == "first" && account.Password == "private-test-value", "Read complete account");
    var replacement = path + ".new";
    await File.WriteAllTextAsync(replacement, JsonSerializer.Serialize(new { username = "second", password = "rotated" }));
    File.Move(replacement, path, overwrite: true);
    account = Sidecar(); await resolver.ApplyAsync(account);
    Check(account.Username == "second" && account.Password == "rotated", "Atomic file rotation is observed on next call");
    foreach (var json in new[] { "not json private-test-value", "{}", "[]", "{\"username\":\"a\",\"password\":null}", "{\"username\":1,\"password\":\"private-test-value\"}", "{\"username\":\"a\",\"password\":\"\"}" })
    {
        await File.WriteAllTextAsync(path, json); await Reject(Sidecar(), "Invalid account");
    }
    await File.WriteAllTextAsync(path, new string('x', 65537)); await Reject(Sidecar(), "Oversized file");
    var insecure = Sidecar(); insecure.UseTls = false;
    await Reject(insecure, "TLS required");
    config.CredentialsFilePath = "relative-account.json"; await Reject(Sidecar(), "Absolute path required");
    using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
    config.CredentialsFilePath = path;
    try { await resolver.ApplyAsync(Sidecar(), cancellation.Token); throw new Exception("Cancellation ignored"); }
    catch (OperationCanceledException) { checks++; }
    Console.WriteLine($"Passed {checks} SMTP credential checks.");
}
finally { Directory.Delete(directory, recursive: true); }

sealed class Monitor(SmtpOptions value) : IOptionsMonitor<SmtpOptions>
{
    public SmtpOptions CurrentValue => value;
    public SmtpOptions Get(string? name) => value;
    public IDisposable? OnChange(Action<SmtpOptions, string?> listener) => null;
}
