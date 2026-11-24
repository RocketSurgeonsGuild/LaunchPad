# RequestAuthorizationPlan

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

The policies a request requires. LaunchPad's generator writes one for every request with a policy attribute, and registers it with the services of the application ( [AuthorizationServiceCollectionExtensions.AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addrequestauthorizationplan-iservicecollection-policygroup)).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public abstract class RequestAuthorizationPlan
{
    // ...
}
```

## Properties

- [Groups](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#groups) : [IReadOnlyList<PolicyGroup>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup.md) `get` -- The requirements, all of which must pass.
- [Id](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#id) : `string` `get` -- The request's full type name.
- [RequestType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#requesttype) : `Type` `get` -- The request.
- [ScopeAlternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#scopealternatives) : `IReadOnlyList<IReadOnlyList<string>>` `get` -- The scopes that satisfy the plan, as alternatives: any one set is enough. A stacked policy adds its scopes to every alternative, a group of alternatives multiplies them; duplicates and alternatives that contain another are dropped.

## Fields

- [MaxAlternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/fields.md#maxalternatives) : `int` `static` -- The most alternatives that are published.
