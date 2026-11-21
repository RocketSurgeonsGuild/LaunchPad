using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

using Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

using ZeroAlloc.Authorization;

namespace Sample.BlazorServer;

/// <summary>
///     The caller the policies of the operations see in a circuit: the user who was signed in when the circuit started. On the server the
///     authentication state is ready by then, so it is read without waiting.
/// </summary>
internal sealed class CircuitSecurityContext(AuthenticationStateProvider authentication) : ISecurityContext
{
    private ClaimsPrincipal? _user;

    public string Id => Current.Id;
    public IReadOnlySet<string> Roles => Current.Roles;
    public IReadOnlyDictionary<string, string> Claims => Current.Claims;

    private ISecurityContext Current
    {
        get
        {
            var state = authentication.GetAuthenticationStateAsync();
            var user = state.IsCompletedSuccessfully ? state.Result.User : null;
            if (ReferenceEquals(user, _user)) return field;
            _user = user;
            return field = ClaimsSecurityContext.From(user);
        }
    } = AnonymousSecurityContext.Instance;
}
