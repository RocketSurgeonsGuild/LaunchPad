namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     On a <c>[Policy]</c> class: scopes the policy requires of the caller, all of them. The policy is then what its scope attributes say and
///     nothing else: LaunchPad writes its <c>EvaluateAsync</c>, which checks the caller's scopes, and states the scopes as those of every
///     operation whose request names the policy.
/// </summary>
/// <remarks>
///     The class is partial and lists <c>IAuthorizationPolicy</c>. Several attributes on a class must all be satisfied, alongside its
///     <see cref="RequireAnyScopeAttribute" />s.
/// </remarks>
/// <param name="scopes">The scopes, as they appear in the caller's <c>scope</c> claim.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RequireScopeAttribute(params string[] scopes) : Attribute
{
    /// <summary>The scopes, all of which the caller must have.</summary>
    public string[] Scopes { get; } = scopes;
}

/// <summary>
///     On a <c>[Policy]</c> class: scopes of which the policy requires the caller to have at least one. See
///     <see cref="RequireScopeAttribute" />.
/// </summary>
/// <param name="scopes">The scopes, as they appear in the caller's <c>scope</c> claim.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RequireAnyScopeAttribute(params string[] scopes) : Attribute
{
    /// <summary>The scopes, any one of which is enough.</summary>
    public string[] Scopes { get; } = scopes;
}
