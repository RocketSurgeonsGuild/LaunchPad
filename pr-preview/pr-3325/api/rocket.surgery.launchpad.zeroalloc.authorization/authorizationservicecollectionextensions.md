# AuthorizationServiceCollectionExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1296`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/AuthorizationServiceCollectionExtensions.cs)

Registers what authorizes requests by their policies. LaunchPad's generator writes the calls for the requests of each project, into the `AddLaunchPadRequestAuthorization()` its joint applies; an application without Dovetail calls that method itself.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public static class AuthorizationServiceCollectionExtensions
{
    // ...
}
```

## Methods

- [AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse) : `IServiceCollection` `extension` -- Registers how a denial becomes a response, unless the response has it registered.
- [AddLaunchPadAuthorization(IServiceCollection)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addlaunchpadauthorization-iservicecollection) : `IServiceCollection` `extension` -- Registers what every plan needs: the [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md) and the [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) of each call, the [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md) of the application and the [RequestAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationoptions.md).
- [AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addrequestauthorizationplan-iservicecollection-policygroup) : `IServiceCollection` `extension` -- Registers the plan of a request, unless it has one.
