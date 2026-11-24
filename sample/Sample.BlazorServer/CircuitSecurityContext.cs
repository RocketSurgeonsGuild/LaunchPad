using Microsoft.AspNetCore.Components.Authorization;

using Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

using ZeroAlloc.Authorization;

namespace Sample.BlazorServer;

/// <summary>
///     The caller the policies of the operations see in a circuit: the circuit's signed-in user, kept up to date when the authentication
///     state changes. On the server the state is ready when the circuit starts, so it is read without waiting.
/// </summary>
internal sealed class CircuitSecurityContext : ISecurityContext, IDisposable
{
    private readonly AuthenticationStateProvider _authentication;

    public CircuitSecurityContext(AuthenticationStateProvider authentication)
    {
        _authentication = authentication;
        _authentication.AuthenticationStateChanged += OnChanged;
    }

    public string Id => Current.Id;
    public IReadOnlySet<string> Roles => Current.Roles;
    public IReadOnlyDictionary<string, string> Claims => Current.Claims;

    [AllowNull]
    private ISecurityContext Current
    {
        get
        {
            if (field is { } current) return current;
            // A state that is not ready yet is nobody, and is asked for again at the next read.
            var state = _authentication.GetAuthenticationStateAsync();
            return state.IsCompletedSuccessfully ? field = ClaimsSecurityContext.From(state.Result.User) : AnonymousSecurityContext.Instance;
        }

        set;
    }

    public void Dispose() => _authentication.AuthenticationStateChanged -= OnChanged;

    private void OnChanged(Task<AuthenticationState> state) =>
        Current = state.IsCompletedSuccessfully ? ClaimsSecurityContext.From(state.Result.User) : null;
}
