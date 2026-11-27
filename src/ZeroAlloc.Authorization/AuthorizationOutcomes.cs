using ZeroAlloc.Authorization;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     What each policy decided for the current call, by policy name and arguments. The edge and the mediator pipeline share it through the
///     call's service scope, so a policy runs once.
/// </summary>
[PublicAPI]
public sealed class AuthorizationOutcomes
{
    private readonly Dictionary<string, PolicyOutcome> _outcomes = [with(StringComparer.Ordinal)];

    /// <summary>The outcome recorded for a policy, if it has run for this call.</summary>
    /// <param name="policy">The policy.</param>
    /// <param name="outcome">Its outcome.</param>
    public bool TryGet(PolicyRequirement policy, out PolicyOutcome outcome)
    {
        ArgumentNullException.ThrowIfNull(policy);
        lock (_outcomes) return _outcomes.TryGetValue(policy.Key, out outcome);
    }

    /// <summary>Records what a policy decided for this call.</summary>
    /// <param name="policy">The policy.</param>
    /// <param name="outcome">Its outcome.</param>
    public void Record(PolicyRequirement policy, PolicyOutcome outcome)
    {
        ArgumentNullException.ThrowIfNull(policy);
        lock (_outcomes) _outcomes[policy.Key] = outcome;
    }
}

/// <summary>What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.</summary>
/// <param name="Allowed">The policy allows the caller.</param>
/// <param name="MissingScopes">The declared scopes the caller lacks; empty when the policy's own logic decided.</param>
/// <param name="Failure">The policy's own failure, when its logic denied the caller.</param>
[PublicAPI]
public readonly record struct PolicyOutcome(bool Allowed, IReadOnlyList<string> MissingScopes, AuthorizationFailure? Failure)
{
    /// <summary>The policy allows the caller.</summary>
    public static PolicyOutcome Success { get; } = new(true, [], null);
}
