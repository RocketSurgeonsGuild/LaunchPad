# RequestAuthorizer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestAuthorizer
{
    // ...
}
```

## Constructors

- [RequestAuthorizer(IServiceProvider, AuthorizationOutcomes, RequestAuthorizationPlans, IOptions<RequestAuthorizationOptions>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/constructors.md#constructor-iserviceprovider-authorizationoutcomes-requestauthorizationplans-ioptions-requestauthorizationoptions) -- Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once.

## Properties

- [Caller](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/properties.md#caller) : `ISecurityContext` `get` -- The caller of this call; anonymous when the application registered no `Authorization.ISecurityContext`.

## Methods

- [AuthorizeAsync(RequestAuthorizationPlan<TRequest>, TRequest, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#authorizeasync-requestauthorizationplan-trequest-trequest-cancellationtoken) : `ValueTask<AuthorizationDecision>` -- Decides the plan with the request: every policy that has no outcome yet runs.
- [AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#authorizeattheedgeasync-requestauthorizationplan-cancellationtoken) : `ValueTask<AuthorizationDecision>` -- Decides the plan at the edge, before there is a request. A policy that reads the request is left for the pipeline, so the decision denies only what is certain.
- [IsAnonymous(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#isanonymous-isecuritycontext) : `bool` `static` -- Whether the caller has no identity.
