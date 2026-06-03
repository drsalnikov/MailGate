using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace MailGate.Api.Auth;

public sealed class KeycloakRoleClaimsTransformation(IConfiguration configuration) : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity source || !source.IsAuthenticated)
            return Task.FromResult(principal);

        // JWT handler часто задаёт RoleClaimType не как ClaimTypes.Role; IsInRole смотрит только RoleClaimType.
        var identity = new ClaimsIdentity(
            source.Claims,
            source.AuthenticationType,
            source.NameClaimType,
            ClaimTypes.Role);

        AddJwtRolesClaim(principal, identity);
        AddRealmRolesFromJson(principal, identity);
        AddRealmRolesFromFlatClaims(principal, identity);
        AddClientRolesFromJson(principal, identity);
        AddClientRolesFromFlatClaims(principal, identity);

        return Task.FromResult(new ClaimsPrincipal(identity));
    }

    private static void AddJwtRolesClaim(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        foreach (var claim in principal.FindAll("roles"))
        {
            if (string.IsNullOrWhiteSpace(claim.Value))
                continue;

            if (claim.Value.TrimStart().StartsWith('['))
            {
                TryAddRolesFromJsonString(identity, claim.Value);
                continue;
            }

            AddRole(identity, claim.Value);
        }
    }

    private static void AddRealmRolesFromJson(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        var realmAccess = principal.FindFirst("realm_access")?.Value;
        if (string.IsNullOrWhiteSpace(realmAccess))
            return;

        using var document = JsonDocument.Parse(realmAccess);
        if (!document.RootElement.TryGetProperty("roles", out var roles))
            return;

        AddRolesFromJsonElement(identity, roles);
    }

    /// <summary>
    /// Keycloak при claim.name вида realm_access.roles может отдавать отдельные claim'ы с таким типом.
    /// </summary>
    private static void AddRealmRolesFromFlatClaims(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        foreach (var claim in principal.FindAll("realm_access.roles"))
            AddRole(identity, claim.Value);
    }

    private void AddClientRolesFromJson(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        var resourceAccess = principal.FindFirst("resource_access")?.Value;
        if (string.IsNullOrWhiteSpace(resourceAccess))
            return;

        var clientId = configuration["Authentication:Keycloak:ClientId"] ?? "mailgate-admin";

        using var document = JsonDocument.Parse(resourceAccess);
        if (!document.RootElement.TryGetProperty(clientId, out var client) ||
            !client.TryGetProperty("roles", out var roles))
        {
            return;
        }

        AddRolesFromJsonElement(identity, roles);
    }

    private void AddClientRolesFromFlatClaims(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        var clientId = configuration["Authentication:Keycloak:ClientId"] ?? "mailgate-admin";
        var claimType = $"resource_access.{clientId}.roles";

        foreach (var claim in principal.FindAll(claimType))
            AddRole(identity, claim.Value);
    }

    private static void TryAddRolesFromJsonString(ClaimsIdentity identity, string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            AddRolesFromJsonElement(identity, document.RootElement);
        }
        catch (JsonException)
        {
            // ignore
        }
    }

    private static void AddRolesFromJsonElement(ClaimsIdentity identity, JsonElement roles)
    {
        if (roles.ValueKind != JsonValueKind.Array)
            return;

        foreach (var role in roles.EnumerateArray())
        {
            var value = role.GetString();
            AddRole(identity, value);
        }
    }

    private static void AddRole(ClaimsIdentity identity, string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || identity.HasClaim(ClaimTypes.Role, value))
            return;

        identity.AddClaim(new Claim(ClaimTypes.Role, value));
    }
}
