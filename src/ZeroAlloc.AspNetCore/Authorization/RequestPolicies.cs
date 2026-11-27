using System.Collections.Concurrent;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Authorization;

using AuthorizationOptions = Microsoft.AspNetCore.Authorization.AuthorizationOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

/// <summary>The requirement of an edge policy: one policy a request names, checked before the request is bound.</summary>
/// <param name="policy">The policy.</param>
internal sealed class RequestPolicyRequirement(PolicyRequirement policy) : IAuthorizationRequirement
{
    public PolicyRequirement Policy { get; } = policy;

    public override string ToString() => $"The policy {Policy.Key}";
}

/// <summary>
///     Answers the name of a policy some request names (<c>[RequirePolicy]</c>, <c>[RequireAnyPolicy]</c>) with an ASP.NET Core policy that
///     runs it: the generated servers put those names on an operation whose request has policies. Every other name is the application's own.
/// </summary>
/// <remarks>
///     A request's policy and an ASP.NET Core policy of the application with the same name are one name for two things: the request's policy
///     is the one that answers. Give them different names.
/// </remarks>
internal sealed class RequestPolicyProvider(IOptions<AuthorizationOptions> options, RequestAuthorizationPlans plans) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies = new(StringComparer.Ordinal);

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) =>
        plans.FindPolicy(policyName) is { } policy
            ? Task.FromResult<AuthorizationPolicy?>(_policies.GetOrAdd(policyName, static (_, policy) => new AuthorizationPolicyBuilder().AddRequirements(new RequestPolicyRequirement(policy)).Build(), policy))
            : base.GetPolicyAsync(policyName);
}

/// <summary>
///     Checks a policy at the edge, with the services of the HTTP request, so the outcome it records is the one the mediator pipeline finds.
///     The decision is kept on the <see cref="HttpContext" /> for the answer.
/// </summary>
internal sealed class RequestPolicyHandler(IHttpContextAccessor accessor) : AuthorizationHandler<RequestPolicyRequirement>
{
    private static readonly object DecisionKey = new();

    /// <summary>What the policies of the endpoint denied the caller, if any did: the first denial, with the scopes of them all.</summary>
    public static AuthorizationDecision? Decision(HttpContext context) =>
        context.Items.TryGetValue(DecisionKey, out var value) && value is AuthorizationDecision decision ? decision : null;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequestPolicyRequirement requirement)
    {
        if (( context.Resource as HttpContext ?? accessor.HttpContext ) is not { } http)
        {
            context.Fail(new(this, $"{requirement} has no HTTP request to check it for."));
            return;
        }

        var decision = await http.RequestServices.GetRequiredService<RequestAuthorizer>().AuthorizeAsync(requirement.Policy, http.RequestAborted).ConfigureAwait(false);
        if (decision.Problem is not { } problem)
        {
            context.Succeed(requirement);
            return;
        }

        // An endpoint names each of its policies, and each that denies is asked: the answer is the first denial, challenging for the scopes
        // of them all.
        http.Items[DecisionKey] = Decision(http) is { } earlier ? earlier with { RequiredScopes = [.. earlier.RequiredScopes.Union(decision.RequiredScopes, StringComparer.Ordinal)] } : decision;
        context.Fail(new(this, problem.Detail ?? problem.GetType().Name));
    }
}
