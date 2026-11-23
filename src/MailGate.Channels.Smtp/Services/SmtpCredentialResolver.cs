using System.Text.Json;
using MailGate.Channels.Smtp.Options;
using MailGate.Core.Enums;
using Microsoft.Extensions.Options;

namespace MailGate.Channels.Smtp.Services;

/// <summary>Reads one atomically rendered credential file; never falls back on failure.</summary>
public sealed class SmtpCredentialResolver(IOptionsMonitor<SmtpOptions> settings)
{
    public async Task ApplyAsync(SmtpOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (options.CredentialSource == SmtpCredentialSource.Legacy) return;
        options.Username = null;
        options.Password = null;
        options.OAuth2 = null;
        if (options.CredentialSource == SmtpCredentialSource.Manual) return;
        if (options.CredentialSource != SmtpCredentialSource.Sidecar)
            throw new InvalidOperationException("Неизвестный источник учётных данных SMTP.");
        if (!options.UseTls && !options.UseSsl)
            throw new InvalidOperationException("Для почтовой учётной записи включите STARTTLS или SSL.");
        var path = settings.CurrentValue.CredentialsFilePath;
        try
        {
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
                throw new IOException();
            await using var file = new FileStream(path, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.Asynchronous);
            if (file.Length is <= 0 or > 65536) throw new IOException();
            // Bound the stream even if a producer writes in-place. Atomic rename is required.
            var buffer = new byte[65537];
            var count = 0;
            while (count < buffer.Length)
            {
                var read = await file.ReadAsync(buffer.AsMemory(count), ct);
                if (read == 0) break;
                count += read;
            }
            if (count > 65536) throw new IOException();
            using var document = JsonDocument.Parse(buffer.AsMemory(0, count));
            var root = document.RootElement;
            var username = root.GetProperty("username").GetString();
            var password = root.GetProperty("password").GetString();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)
                || username.Length > 4096 || password.Length > 16384)
                throw new IOException();
            options.Username = username;
            options.Password = password;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException
            or KeyNotFoundException or InvalidOperationException or ArgumentException)
        {
            // Never include the file, its path or parser exceptions in user-facing logs.
            throw new InvalidOperationException("Не удалось прочитать SMTP-учётную запись из файла. Проверьте наличие файла, права доступа и поля username/password.");
        }
    }
}
