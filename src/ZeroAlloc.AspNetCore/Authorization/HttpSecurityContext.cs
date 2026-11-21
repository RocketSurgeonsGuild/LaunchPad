using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using ZeroAlloc.Authorization;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

/// <summary>
///     The caller of the current HTTP request as policies see them: read from <see cref="HttpContext.User" /> when asked, so it is the caller
///     authentication found, whenever the context was resolved.
/// </summary>
internal sealed class HttpSecurityContext(IHttpContextAccessor accessor) : ISecurityContext
{
    private ClaimsPrincipal? _user;

    public string Id => Current.Id;
    public IReadOnlySet<string> Roles => Current.Roles;
    public IReadOnlyDictionary<string, string> Claims => Current.Claims;

    private ISecurityContext Current
    {
        get
        {
            var user = accessor.HttpContext?.User;
            if (ReferenceEquals(user, _user)) return field;
            _user = user;
            return field = ClaimsSecurityContext.From(user);
        }
    } = AnonymousSecurityContext.Instance;
}

/// <summary>A <see cref="ClaimsPrincipal" /> as a security context.</summary>
[PublicAPI]
public sealed class ClaimsSecurityContext : ISecurityContext
{
    // What names the caller, in order: the subject as ASP.NET Core maps it and as the token names it, then what names a client without a user.
    private static readonly string[] IdClaims = [ClaimTypes.NameIdentifier, "sub", "client_id", "azp", "appid"];
    private static readonly string[] RoleClaims = [ClaimTypes.Role, "role", "roles"];

    private ClaimsSecurityContext(string id, IReadOnlySet<string> roles, IReadOnlyDictionary<string, string> claims)
    {
        Id = id;
        Roles = roles;
        Claims = claims;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public IReadOnlySet<string> Roles { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Claims { get; }

    /// <summary>
    ///     The principal as a security context: its subject as the id, its role claims as roles, and its claims by type, the values of a
    ///     repeated claim joined by a space. A principal that is not authenticated is the anonymous security context.
    /// </summary>
    /// <param name="user">The principal.</param>
    public static ISecurityContext From(ClaimsPrincipal? user)
    {
        if (user?.Identities.Any(i => i.IsAuthenticated) is not true) return AnonymousSecurityContext.Instance;

        var claims = new Dictionary<string, string>(StringComparer.Ordinal);
        var roles = new HashSet<string>(StringComparer.Ordinal);
        foreach (var identity in user.Identities)
        {
            foreach (var claim in identity.Claims)
            {
                claims[claim.Type] = claims.TryGetValue(claim.Type, out var value) ? value + " " + claim.Value : claim.Value;
                if (claim.Type == identity.RoleClaimType || Array.IndexOf(RoleClaims, claim.Type) >= 0) roles.Add(claim.Value);
            }
        }

        string? id = null;
        foreach (var type in IdClaims)
        {
            if (user.FindFirst(type)?.Value is { Length: > 0 } found)
            {
                id = found;
                break;
            }
        }

        // An authenticated caller is never taken for an anonymous one for want of a subject.
        return new ClaimsSecurityContext(id ?? user.Identity?.Name ?? "authenticated", roles, claims);
    }
}
