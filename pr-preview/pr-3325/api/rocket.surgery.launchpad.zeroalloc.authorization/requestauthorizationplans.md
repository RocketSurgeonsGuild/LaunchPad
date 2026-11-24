# RequestAuthorizationPlans

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestAuthorizationPlans
{
    // ...
}
```

## Constructors

- [RequestAuthorizationPlans(IEnumerable<RequestAuthorizationPlan>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/constructors.md#constructor-ienumerable-requestauthorizationplan) -- The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.

## Properties

- [All](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/properties.md#all) : [IReadOnlyCollection<RequestAuthorizationPlan>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md) `get` -- Every plan of the application.

## Methods

- [Find(Type)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/methods.md#find-type) : [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md) -- The plan of a request, if it has policies.
- [FindPolicy(string?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/methods.md#findpolicy-string) : [PolicyRequirement?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md) -- A policy some request names, by its [PolicyRequirement.Key](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#key): the policy's name, with the arguments the request passes it. Null for a name no request names, which is then a policy of the application's own.
