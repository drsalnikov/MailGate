using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace MailGate.Admin.Auth;

/// <summary>
/// Keycloak часто не кладёт <c>realm_access</c> в ID token (только профиль), а realm-роли остаются в <b>access token</b>.
/// Blazor проверяет <see cref="AuthorizeAttribute.Roles"/> по <see cref="RemoteAuthenticationUserOptions.RoleClaim"/> на principal из ID token —
/// поэтому подмешиваем роли из access token (только чтение payload, без доверия подписи на клиенте; API по-прежнему валидирует Bearer).
/// </summary>
public sealed class KeycloakAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor)
    : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor)
{
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var principal = await base.CreateUserAsync(account, options);
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return principal;

        var roleClaimType = string.IsNullOrWhiteSpace(options.RoleClaim)
            ? ClaimTypes.Role
            : options.RoleClaim;

        AddRealmAccessRoles(identity, roleClaimType);
        AddResourceAccessRoles(identity, roleClaimType);
        NormalizeRolesJsonArrayClaim(identity, roleClaimType);
        AddRolesFromRemoteUserAccount(account, identity, roleClaimType);
        await TryAddRolesFromAccessTokenAsync(identity, roleClaimType);

        return principal;
    }

    private static void AddRolesFromRemoteUserAccount(
        RemoteUserAccount? account,
        ClaimsIdentity identity,
        string roleClaimType)
    {
        if (account?.AdditionalProperties is null || account.AdditionalProperties.Count == 0)
            return;

        if (TryGetAdditionalRaw(account, "realm_access", out var realmJson))
            TryAddRolesFromRealmAccessJson(identity, roleClaimType, realmJson);

        if (TryGetAdditionalRaw(account, "resource_access", out var resourceJson))
            AddResourceAccessRolesFromJson(identity, roleClaimType, resourceJson);

        foreach (var kv in account.AdditionalProperties)
        {
            if (kv.Key.Equals("realm_access.roles", StringComparison.OrdinalIgnoreCase))
            {
                var v = AdditionalPropertyToString(kv.Value);
                if (!string.IsNullOrWhiteSpace(v))
                    AddRoleClaim(identity, roleClaimType, v);
            }
        }
    }

    private static bool TryGetAdditionalRaw(RemoteUserAccount account, string key, out string json)
    {
        foreach (var kv in account.AdditionalProperties!)
        {
            if (!kv.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                continue;

            var s = AdditionalPropertyToString(kv.Value);
            if (!string.IsNullOrWhiteSpace(s))
            {
                json = s;
                return true;
            }
        }

        json = "";
        return false;
    }

    private static string? AdditionalPropertyToString(object? value) =>
        value switch
        {
            null => null,
            JsonElement je => je.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null ? null : je.GetRawText(),
            string s => s,
            _ => value.ToString()
        };

    private static void AddResourceAccessRolesFromJson(ClaimsIdentity identity, string roleClaimType, string raw)
    {
        try
        {
            using var doc = JsonDocument.Parse(raw);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (!prop.Value.TryGetProperty("roles", out var roles) || roles.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var role in roles.EnumerateArray())
                    AddRoleClaim(identity, roleClaimType, GetRoleName(role));
            }
        }
        catch (JsonException)
        {
            // ignore
        }
    }

    private static void AddRealmAccessRoles(ClaimsIdentity identity, string roleClaimType)
    {
        var raw = identity.FindFirst("realm_access")?.Value;
        if (string.IsNullOrWhiteSpace(raw))
            return;

        TryAddRolesFromRealmAccessJson(identity, roleClaimType, raw);
    }

    private static void AddResourceAccessRoles(ClaimsIdentity identity, string roleClaimType)
    {
        var raw = identity.FindFirst("resource_access")?.Value;
        if (string.IsNullOrWhiteSpace(raw))
            return;

        AddResourceAccessRolesFromJson(identity, roleClaimType, raw);
    }

    /// <summary>
    /// Если в токене один claim <c>roles</c> со значением JSON-массива строк.
    /// </summary>
    private static void NormalizeRolesJsonArrayClaim(ClaimsIdentity identity, string roleClaimType)
    {
        var raw = identity.FindFirst("roles")?.Value;
        if (string.IsNullOrWhiteSpace(raw) || !raw.TrimStart().StartsWith('['))
            return;

        TryAddRolesFromRealmAccessJson(identity, roleClaimType, raw);
    }

    private static void TryAddRolesFromRealmAccessJson(ClaimsIdentity identity, string roleClaimType, string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in doc.RootElement.EnumerateArray())
                {
                    var name = GetRoleName(role);
                    AddRoleClaim(identity, roleClaimType, name);
                }

                return;
            }

            if (doc.RootElement.TryGetProperty("roles", out var roles) &&
                roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var name = GetRoleName(role);
                    AddRoleClaim(identity, roleClaimType, name);
                }
            }
        }
        catch (JsonException)
        {
            // ignore
        }
    }

    /// <summary>
    /// Достаёт роли из access token (там Keycloak обычно кладёт <c>realm_access</c>).
    /// </summary>
    private async Task TryAddRolesFromAccessTokenAsync(ClaimsIdentity identity, string roleClaimType)
    {
        try
        {
            var result = await TokenProvider.RequestAccessToken();
            if (!result.TryGetToken(out var token) || string.IsNullOrWhiteSpace(token.Value))
                return;

            var payloadJson = TryDecodeJwtPayload(token.Value);
            if (string.IsNullOrWhiteSpace(payloadJson))
                return;

            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("realm_access", out var ra) &&
                ra.ValueKind == JsonValueKind.Object &&
                ra.TryGetProperty("roles", out var realmRoles) &&
                realmRoles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in realmRoles.EnumerateArray())
                    AddRoleClaim(identity, roleClaimType, GetRoleName(role));
            }

            if (root.TryGetProperty("resource_access", out var re) && re.ValueKind == JsonValueKind.Object)
                AddResourceAccessRolesFromJson(identity, roleClaimType, re.GetRawText());

            if (root.TryGetProperty("roles", out var topRoles) && topRoles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in topRoles.EnumerateArray())
                    AddRoleClaim(identity, roleClaimType, GetRoleName(role));
            }
        }
        catch
        {
            // не ломаем логин
        }
    }

    private static string? TryDecodeJwtPayload(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return null;

        try
        {
            return Base64UrlDecodeToUtf8(parts[1]);
        }
        catch
        {
            return null;
        }
    }

    private static string Base64UrlDecodeToUtf8(string segment)
    {
        var s = segment.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2:
                s += "==";
                break;
            case 3:
                s += "=";
                break;
        }

        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }

    private static string? GetRoleName(JsonElement role)
    {
        if (role.ValueKind == JsonValueKind.String)
            return role.GetString();

        if (role.ValueKind != JsonValueKind.Object)
            return null;

        foreach (var propertyName in new[] { "name", "role", "value" })
        {
            if (role.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.String)
            {
                return property.GetString();
            }
        }

        return null;
    }

    private static void AddRoleClaim(ClaimsIdentity identity, string roleClaimType, string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName) || identity.HasClaim(roleClaimType, roleName))
            return;

        identity.AddClaim(new Claim(roleClaimType, roleName));
    }
}
