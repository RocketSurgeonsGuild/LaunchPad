# RequestAuthorizer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

Decides a request's plan for a caller, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. The mediator pipeline and the edge both decide through it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public static class RequestAuthorizer
{
    // ...
}
```

## Methods

- [AuthorizeAsync(RequestAuthorizationPlan<TRequest>, TRequest, IServiceProvider, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#authorizeasync-requestauthorizationplan-trequest-trequest-iserviceprovider-cancellationtoken) : `ValueTask<AuthorizationDecision>` `static` -- Decides the plan with the request: every policy that has no outcome yet runs.
- [AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan, IServiceProvider, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#authorizeattheedgeasync-requestauthorizationplan-iserviceprovider-cancellationtoken) : `ValueTask<AuthorizationDecision>` `static` -- Decides the plan at the edge, before there is a request. A policy that reads the request is left for the pipeline, so the decision denies only what is certain.
- [IsAnonymous(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#isanonymous-isecuritycontext) : `bool` `static` -- Whether the caller has no identity.
- [SecurityContext(IServiceProvider)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#securitycontext-iserviceprovider) : `ISecurityContext` `static` -- The caller of the call these services belong to; anonymous when none is registered.
