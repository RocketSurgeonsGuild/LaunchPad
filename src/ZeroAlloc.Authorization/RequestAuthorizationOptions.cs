namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>How LaunchPad reads what a caller was granted.</summary>
[PublicAPI]
public sealed class RequestAuthorizationOptions
{
    /// <summary>
    ///     The claims of the security context that hold the caller's scopes, each a space separated list. <c>scope</c> (OAuth 2.0) and
    ///     <c>scp</c> (Microsoft Entra), under that name or the claim type ASP.NET Core maps it to, unless changed.
    /// </summary>
    public IList<string> ScopeClaims { get; } = ["scope", "scp", "http://schemas.microsoft.com/identity/claims/scope"];
}
