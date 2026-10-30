namespace MailGate.Core.Models;

/// <summary>Bounded raster images stored as data URLs inside template JSON.</summary>
public static class EmbeddedEmailImage
{
    public const int MaxBytes = 128 * 1024;
    public const int MaxTotalBytes = 512 * 1024;

    public static bool TryRead(string value, out string mimeType, out byte[] bytes)
    {
        mimeType = string.Empty;
        bytes = [];
        if (value.Length > MaxBytes * 4 / 3 + 100) return false;
        foreach (var type in new[] { "image/png", "image/jpeg", "image/gif" })
        {
            var prefix = $"data:{type};base64,";
            if (!value.StartsWith(prefix, StringComparison.Ordinal)) continue;
            try { bytes = Convert.FromBase64String(value[prefix.Length..]); }
            catch (FormatException) { return false; }
            if (bytes.Length is < 10 or > MaxBytes) return false;
            // The SMTP converter expects canonical base64 without whitespace.
            if (value[prefix.Length..] != Convert.ToBase64String(bytes)) return false;
            var valid = type switch
            {
                "image/png" => bytes.AsSpan().StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                "image/jpeg" => bytes[0] == 255 && bytes[1] == 216 && bytes[2] == 255,
                _ => bytes.AsSpan().StartsWith("GIF87a"u8) || bytes.AsSpan().StartsWith("GIF89a"u8)
            };
            if (!valid) return false;
            mimeType = type;
            return true;
        }
        return false;
    }
}
