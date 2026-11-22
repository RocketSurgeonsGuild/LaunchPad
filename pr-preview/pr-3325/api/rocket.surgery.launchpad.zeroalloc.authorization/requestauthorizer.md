# RequestAuthorizer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1296`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestAuthorizer
{
    // ...
}
```

## Constructors

- [RequestAuthorizer(IServiceProvider, AuthorizationOutcomes, IOptions<RequestAuthorizationOptions>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/constructors.md#constructor-iserviceprovider-authorizationoutcomes-ioptions-requestauthorizationoptions) -- Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs.

## Properties

- [Caller](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/properties.md#caller) : `ISecurityContext` `get` -- The caller of this call; anonymous when the application registered no `Authorization.ISecurityContext`.

## Methods

- [AuthorizeAsync(RequestAuthorizationPlan, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#authorizeasync-requestauthorizationplan-cancellationtoken) : `ValueTask<AuthorizationDecision>` -- Decides a request's plan for the caller: every policy that has no outcome yet for this call runs.
- [AuthorizeAsync(PolicyRequirement, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#authorizeasync-policyrequirement-cancellationtoken) : `ValueTask<AuthorizationDecision>` -- Decides one policy for the caller, as the edge does for each policy an endpoint names. Its outcome is recorded, so the plan of the request finds it and the policy doesn't run again.
- [IsAnonymous(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer/methods.md#isanonymous-isecuritycontext) : `bool` `static` -- Whether the caller has no identity.
