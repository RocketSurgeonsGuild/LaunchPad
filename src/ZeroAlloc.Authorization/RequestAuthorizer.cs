using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ZeroAlloc.Authorization;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>What a request's policies decide for a caller.</summary>
/// <param name="Problem">Null when allowed; otherwise <see cref="Unauthorized" /> or <see cref="Forbidden" />.</param>
/// <param name="RequiredScopes">The scopes of the requirement that denied the caller, for the challenge; empty otherwise.</param>
[PublicAPI]
public readonly record struct AuthorizationDecision(Problem? Problem, IReadOnlyList<string> RequiredScopes)
{
    /// <summary>The key of <see cref="Problem.Properties" /> that holds the scopes a forbidden caller lacks, separated by spaces.</summary>
    public const string MissingScopesProperty = "missingScopes";

    /// <summary>The caller is allowed.</summary>
    public static AuthorizationDecision Allowed { get; } = new(null, []);

    /// <summary>The caller is allowed.</summary>
    public bool IsAllowed => Problem is null;
}

/// <summary>
///     Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's
///     <see cref="AuthorizationOutcomes" /> and reusing the ones already there. One for each call: the mediator pipeline and the edge both
///     decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs.
/// </summary>
/// <param name="services">The services of the call, from which the caller and the policies are resolved.</param>
/// <param name="outcomes">What each policy decided for this call.</param>
/// <param name="options">How the caller's scopes are read.</param>
[PublicAPI]
public sealed class RequestAuthorizer(
    IServiceProvider services,
    AuthorizationOutcomes outcomes,
    IOptions<RequestAuthorizationOptions> options
)
{
    private HashSet<string>? _granted;

    /// <summary>The caller of this call; anonymous when the application registered no <see cref="ISecurityContext" />.</summary>
    public ISecurityContext Caller => field ??= services.GetService<ISecurityContext>() ?? AnonymousSecurityContext.Instance;

    /// <summary>Whether the caller has no identity.</summary>
    /// <param name="context">The caller.</param>
    public static bool IsAnonymous(ISecurityContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context is AnonymousSecurityContext
         || string.IsNullOrEmpty(context.Id)
         || string.Equals(context.Id, AnonymousSecurityContext.AnonymousId, StringComparison.Ordinal);
    }

    /// <summary>Decides a request's plan for the caller: every policy that has no outcome yet for this call runs.</summary>
    /// <param name="plan">The request's plan.</param>
    /// <param name="cancellationToken">The cancellation token of the call.</param>
    public async ValueTask<AuthorizationDecision> AuthorizeAsync(RequestAuthorizationPlan plan, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (IsAnonymous(Caller)) return new(new Unauthorized("Authentication is required."), ScopesOf(plan.Groups));

        foreach (var group in plan.Groups)
        {
            var last = PolicyOutcome.Success;
            var allowed = false;
            foreach (var policy in group.Policies)
            {
                last = await OutcomeAsync(policy, cancellationToken).ConfigureAwait(false);
                if (!last.Allowed) continue;
                allowed = true;
                break;
            }

            if (!allowed) return new(Forbid(last), ScopesOf([group]));
        }

        return AuthorizationDecision.Allowed;
    }

    /// <summary>
    ///     Decides one policy for the caller, as the edge does for each policy an endpoint names. Its outcome is recorded, so the plan of the
    ///     request finds it and the policy doesn't run again.
    /// </summary>
    /// <param name="policy">The policy.</param>
    /// <param name="cancellationToken">The cancellation token of the call.</param>
    public async ValueTask<AuthorizationDecision> AuthorizeAsync(PolicyRequirement policy, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(policy);
        if (IsAnonymous(Caller)) return new(new Unauthorized("Authentication is required."), [.. policy.Scopes]);

        var outcome = await OutcomeAsync(policy, cancellationToken).ConfigureAwait(false);
        return outcome.Allowed ? AuthorizationDecision.Allowed : new(Forbid(outcome), [.. policy.Scopes]);
    }

    // What the policy decides for this call: recorded the first time, then reused. The declared scopes are checked before the policy runs.
    private async ValueTask<PolicyOutcome> OutcomeAsync(PolicyRequirement policy, CancellationToken cancellationToken)
    {
        if (outcomes.TryGet(policy, out var outcome)) return outcome;

        var granted = _granted ??= GrantedScopes();
        List<string>? missing = null;
        foreach (var scope in policy.Scopes)
        {
            if (!granted.Contains(scope)) ( missing ??= [] ).Add(scope);
        }

        if (missing is { })
        {
            outcome = new(false, missing, null);
        }
        else
        {
            var result = await policy.Evaluate(services, Caller, cancellationToken).ConfigureAwait(false);
            outcome = result.IsSuccess ? PolicyOutcome.Success : new(false, [], result.Error);
        }

        outcomes.Record(policy, outcome);
        return outcome;
    }

    private HashSet<string> GrantedScopes()
    {
        var granted = new HashSet<string>(StringComparer.Ordinal);
        foreach (var claim in options.Value.ScopeClaims)
        {
            if (!Caller.Claims.TryGetValue(claim, out var value)) continue;
            foreach (var scope in value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) granted.Add(scope);
        }

        return granted;
    }

    private static Forbidden Forbid(PolicyOutcome outcome)
    {
        if (outcome.MissingScopes.Count == 0) return new(outcome.Failure?.Reason ?? "The caller may not do this.");
        var forbidden = new Forbidden("The caller lacks a required scope.");
        forbidden.Properties[AuthorizationDecision.MissingScopesProperty] = string.Join(" ", outcome.MissingScopes);
        return forbidden;
    }

    private static string[] ScopesOf(IReadOnlyList<PolicyGroup> groups)
    {
        var scopes = new List<string>();
        foreach (var group in groups)
        {
            // Of a group of alternatives the first is named: a challenge names one way in.
            if (group.Policies.Count == 0) continue;
            foreach (var scope in group.Policies[0].Scopes)
            {
                if (!scopes.Contains(scope, StringComparer.Ordinal)) scopes.Add(scope);
            }
        }

        return [.. scopes];
    }
}
