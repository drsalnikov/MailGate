using System.Security.Claims;
using System.Text.Json;

namespace MailGate.Api.Auth;

public static class KeycloakGroupAccess
{
    public static bool HasGroup(ClaimsPrincipal principal, string? requiredGroup)
    {
        if (string.IsNullOrWhiteSpace(requiredGroup))
            return false;

        foreach (var claim in principal.FindAll("groups"))
        {
            if (string.Equals(claim.Value, requiredGroup, StringComparison.Ordinal))
                return true;

            if (!claim.Value.TrimStart().StartsWith('['))
                continue;

            try
            {
                using var document = JsonDocument.Parse(claim.Value);
                if (document.RootElement.EnumerateArray().Any(group =>
                    group.ValueKind == JsonValueKind.String &&
                    string.Equals(group.GetString(), requiredGroup, StringComparison.Ordinal)))
                    return true;
            }
            catch (JsonException)
            {
                // Malformed group claims do not grant access.
            }
        }

        return false;
    }
}
