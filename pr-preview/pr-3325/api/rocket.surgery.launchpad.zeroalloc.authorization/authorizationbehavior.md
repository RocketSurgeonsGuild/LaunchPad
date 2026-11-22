# AuthorizationBehavior

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1296`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/AuthorizationBehavior.cs)
- Implements: `IPipelineBehavior`, `IPipelineBehavior`

The ZeroAlloc.Mediator pipeline behavior that checks a request's policies before anything else runs. A caller with no identity answers the `Foundation.Unauthorized` case of the response union, and a caller a policy denies answers `Foundation.Forbidden`; a request without policies passes straight through.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class AuthorizationBehavior
    : ZeroAlloc.Mediator.IPipelineBehavior,
      ZeroAlloc.Pipeline.IPipelineBehavior
{
    // ...
}
```

## Remarks

The plan of a request, and how a denial becomes its response, are registered with the services of the application: LaunchPad's generator writes `AddLaunchPadRequestAuthorization()` for each project, and the joint LaunchPad writes beside it applies it. A request that has a policy attribute and no plan is not let through: it throws, naming the registration that is missing.

The services are the ones the mediator dispatches the request with: `IMediator`, resolved from the scope of the call. ZeroAlloc.Mediator's static `Mediator` class dispatches without any, so LaunchPad doesn't support it and bans it (RS0030); a request with a policy that is dispatched that way is refused.

Policies the edge already decided for this call are not run again: their outcomes are in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md). A stream request has no response to answer with, so its denial throws [AuthorizationDeniedException](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception.md).

## Constructors

- [AuthorizationBehavior](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior/constructors.md#constructor)

## Methods

- [CheckStream(TRequest, IServiceProvider?, CancellationToken)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior/methods.md#checkstream-trequest-iserviceprovider-cancellationtoken) : `ValueTask` `static` -- Checks a stream request's policies before its handler runs; a denial throws [AuthorizationDeniedException](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception.md).
- [Handle(TRequest, CancellationToken, Func<TRequest, CancellationToken, ValueTask<TResponse>>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior/methods.md#handle-trequest-cancellationtoken-func-trequest-cancellationtoken-valuetask-tresponse) : `ValueTask<TResponse>` `static` -- What ZeroAlloc.Mediator's static `Mediator` class calls, which dispatches with no services: its generator writes the call for every behavior, so the overload has to be there. LaunchPad doesn't support that way of dispatching: a request with a policy is refused, and any other request passes on.
- [Handle(TRequest, IServiceProvider, CancellationToken, Func<TRequest, CancellationToken, ValueTask<TResponse>>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior/methods.md#handle-trequest-iserviceprovider-cancellationtoken-func-trequest-cancellationtoken-valuetask-tresponse) : `ValueTask<TResponse>` `static` -- Checks the request's policies with the services of the request being dispatched.
