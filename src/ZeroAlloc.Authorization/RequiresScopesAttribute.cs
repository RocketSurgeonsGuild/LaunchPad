namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     On a <c>[Policy]</c> class: the scopes the policy requires of the caller, all of them. LaunchPad checks them before the policy runs,
///     and publishes them as the scopes of every operation whose request names the policy.
/// </summary>
/// <param name="scopes">The scopes, as they appear in the caller's token.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class RequiresScopesAttribute(params string[] scopes) : Attribute
{
    /// <summary>The scopes, as they appear in the caller's token.</summary>
    public string[] Scopes { get; } = scopes;
}
