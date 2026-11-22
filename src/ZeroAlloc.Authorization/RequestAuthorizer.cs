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
///     decide through it, so a policy runs once.
/// </summary>
/// <param name="services">The services of the call, from which the caller and the policies are resolved.</param>
/// <param name="outcomes">What each policy decided for this call.</param>
/// <param name="plans">The plans of the application.</param>
/// <param name="options">How the caller's scopes are read.</param>
[PublicAPI]
public sealed class RequestAuthorizer(
    IServiceProvider services,
    AuthorizationOutcomes outcomes,
    RequestAuthorizationPlans plans,
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

    /// <summary>Decides the plan with the request: every policy that has no outcome yet runs.</summary>
    /// <typeparam name="TRequest">The request.</typeparam>
    /// <param name="plan">The request's plan.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token of the call.</param>
    public ValueTask<AuthorizationDecision> AuthorizeAsync<TRequest>(RequestAuthorizationPlan<TRequest> plan, TRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return DecideAsync(plan, plan.WithRequest(Caller, request), false, cancellationToken);
    }

    /// <summary>
    ///     Decides the plan at the edge, before there is a request. A policy that reads the request is left for the pipeline, so the
    ///     decision denies only what is certain.
    /// </summary>
    /// <param name="plan">The request's plan.</param>
    /// <param name="cancellationToken">The cancellation token of the call.</param>
    public ValueTask<AuthorizationDecision> AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan plan, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return DecideAsync(plan, plan.AtTheEdge(Caller), true, cancellationToken);
    }

    private async ValueTask<AuthorizationDecision> DecideAsync(RequestAuthorizationPlan plan, ISecurityContext context, bool atTheEdge, CancellationToken cancellationToken)
    {
        if (IsAnonymous(Caller)) return new(new Unauthorized("Authentication is required."), ScopesOf(plan.Groups));

        foreach (var group in plan.Groups)
        {
            var allowed = false;
            var deferred = false;
            var last = PolicyOutcome.Success;
            foreach (var policy in group.Policies)
            {
                if (!outcomes.TryGet(policy, out var outcome))
                {
                    if (await EvaluateAsync(policy, context, atTheEdge, cancellationToken).ConfigureAwait(false) is not { } decided)
                    {
                        deferred = true;
                        continue;
                    }

                    outcome = decided;
                    outcomes.Record(policy, outcome);
                }

                last = outcome;
                if (!outcome.Allowed) continue;
                allowed = true;
                break;
            }

            if (allowed || deferred) continue;
            return new(Forbid(last), ScopesOf([group]));
        }

        return AuthorizationDecision.Allowed;
    }

    // Null: the policy reads the request and there is none here.
    private async ValueTask<PolicyOutcome?> EvaluateAsync(PolicyRequirement policy, ISecurityContext context, bool atTheEdge, CancellationToken cancellationToken)
    {
        var granted = _granted ??= GrantedScopes();
        List<string>? missing = null;
        foreach (var scope in policy.Scopes)
        {
            if (!granted.Contains(scope)) ( missing ??= [] ).Add(scope);
        }

        if (missing is { }) return new PolicyOutcome(false, missing, null);
        if (atTheEdge && plans.ReadsTheRequest(policy.PolicyType)) return null;

        try
        {
            var result = await policy.Evaluate(services, context, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? PolicyOutcome.Success : new PolicyOutcome(false, [], result.Error);
        }
        catch (RequestNotAvailableException) when (atTheEdge)
        {
            plans.MarkReadsTheRequest(policy.PolicyType);
            return null;
        }
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
