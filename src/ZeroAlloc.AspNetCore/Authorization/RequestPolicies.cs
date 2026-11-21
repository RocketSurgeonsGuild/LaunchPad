using System.Collections.Concurrent;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Authorization;

using AuthorizationOptions = Microsoft.AspNetCore.Authorization.AuthorizationOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

/// <summary>The requirement of an edge policy: the policies of one request, checked before the request is bound.</summary>
/// <param name="id">The request's full type name.</param>
/// <param name="plan">The request's plan; null when no request with policies has that name, which denies.</param>
internal sealed class RequestPoliciesRequirement(string id, RequestAuthorizationPlan? plan) : IAuthorizationRequirement
{
    public string Id { get; } = id;
    public RequestAuthorizationPlan? Plan { get; } = plan;

    public override string ToString() => $"The policies of {Id}";
}

/// <summary>
///     Answers the policy names the generated servers put on an operation whose request has policies (<c>launchpad:{request}</c>) with a policy
///     that checks the request's plan. Every other name is the application's own.
/// </summary>
internal sealed class RequestPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies = new(StringComparer.Ordinal);

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) =>
        policyName.StartsWith(RequestAuthorizationPlan.PolicyPrefix, StringComparison.Ordinal)
            ? Task.FromResult<AuthorizationPolicy?>(_policies.GetOrAdd(policyName, Build))
            : base.GetPolicyAsync(policyName);

    private static AuthorizationPolicy Build(string policyName)
    {
        var id = policyName[RequestAuthorizationPlan.PolicyPrefix.Length..];
        return new AuthorizationPolicyBuilder().AddRequirements(new RequestPoliciesRequirement(id, RequestAuthorizationPlan.Find(id))).Build();
    }
}

/// <summary>
///     Checks a request's plan at the edge, with the services of the HTTP request, so the outcomes it records are the ones the mediator
///     pipeline finds. The decision is kept on the <see cref="HttpContext" /> for the answer.
/// </summary>
internal sealed class RequestPoliciesHandler(IHttpContextAccessor accessor) : AuthorizationHandler<RequestPoliciesRequirement>
{
    private static readonly object DecisionKey = new();

    public static AuthorizationDecision? Decision(HttpContext context) =>
        context.Items.TryGetValue(DecisionKey, out var value) && value is AuthorizationDecision decision ? decision : null;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequestPoliciesRequirement requirement)
    {
        var http = context.Resource as HttpContext ?? accessor.HttpContext;
        if (http is null || requirement.Plan is not { } plan)
        {
            context.Fail(new(this, $"{requirement.Id} has no authorization plan to check."));
            return;
        }

        var decision = await RequestAuthorizer.AuthorizeAtTheEdgeAsync(plan, http.RequestServices, http.RequestAborted).ConfigureAwait(false);
        if (decision.Problem is not { } problem)
        {
            context.Succeed(requirement);
            return;
        }

        http.Items[DecisionKey] = decision;
        context.Fail(new(this, problem.Detail ?? problem.GetType().Name));
    }
}
