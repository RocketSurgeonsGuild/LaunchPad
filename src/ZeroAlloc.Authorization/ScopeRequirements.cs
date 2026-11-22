using ZeroAlloc.Authorization;
using ZeroAlloc.Results;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>One scope attribute of a policy: scopes the caller must have all of, or any one of.</summary>
/// <param name="any">Whether any one of the scopes is enough.</param>
/// <param name="scopes">The scopes.</param>
[PublicAPI]
public sealed class ScopeRequirement(bool any, params string[] scopes)
{
    /// <summary>Whether any one of the scopes is enough; otherwise the caller must have them all.</summary>
    public bool Any { get; } = any;

    /// <summary>The scopes.</summary>
    public IReadOnlyList<string> Scopes { get; } = scopes;
}

/// <summary>
///     Checks the scopes of a caller: what the <c>EvaluateAsync</c> LaunchPad writes for a policy with scope attributes calls. A policy is
///     checked wherever its request is dispatched, with or without a transport, so the check is the policy's own.
/// </summary>
[PublicAPI]
public static class ScopeRequirements
{
    /// <summary>The claim of the security context that holds the caller's scopes, separated by spaces.</summary>
    public const string ScopeClaim = "scope";

    /// <summary>
    ///     The code of the failure a caller without a required scope is denied with. Its reason is the scopes the caller lacks, separated by
    ///     spaces.
    /// </summary>
    public const string MissingScopeCode = "scope.missing";

    /// <summary>The scopes the caller has.</summary>
    /// <param name="context">The caller.</param>
    public static IReadOnlySet<string> Granted(ISecurityContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Claims.TryGetValue(ScopeClaim, out var scope)
            ? new HashSet<string>(scope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), StringComparer.Ordinal)
            : Empty;
    }

    /// <summary>
    ///     Whether the caller satisfies every requirement; otherwise a failure with <see cref="MissingScopeCode" /> that names the scopes of
    ///     the first requirement the caller doesn't satisfy.
    /// </summary>
    /// <param name="context">The caller.</param>
    /// <param name="requirements">The scope attributes of the policy.</param>
    public static UnitResult<AuthorizationFailure> Evaluate(ISecurityContext context, params ScopeRequirement[] requirements)
    {
        ArgumentNullException.ThrowIfNull(requirements);
        var granted = Granted(context);
        foreach (var requirement in requirements)
        {
            if (Missing(requirement, granted) is { } missing) return UnitResult<AuthorizationFailure>.Failure(new(MissingScopeCode, missing));
        }

        return UnitResult<AuthorizationFailure>.Success();
    }

    // The scopes of the requirement the caller lacks, when it doesn't satisfy it: any one is enough for an "any" requirement, so the caller
    // lacks it only when it lacks them all.
    private static string? Missing(ScopeRequirement requirement, IReadOnlySet<string> granted)
    {
        var missing = requirement.Scopes.Where(scope => !granted.Contains(scope)).ToArray();
        var satisfied = requirement.Any ? missing.Length < requirement.Scopes.Count || missing.Length == 0 : missing.Length == 0;
        return satisfied ? null : string.Join(' ', missing);
    }

    private static readonly HashSet<string> Empty = [with(StringComparer.Ordinal)];
}
