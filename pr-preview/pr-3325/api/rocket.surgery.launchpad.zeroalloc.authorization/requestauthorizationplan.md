# RequestAuthorizationPlan

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

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
- [Id](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#id) : `string` `get` -- What names the plan at the edge: the request's full type name.
- [PolicyName](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#policyname) : `string` `get` -- The name of the edge policy that checks this plan.
- [RequestType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#requesttype) : `Type` `get` -- The request.
- [ScopeAlternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#scopealternatives) : `IReadOnlyList<IReadOnlyList<string>>` `get` -- The scopes that satisfy the plan, as alternatives: any one set is enough. A stacked policy adds its scopes to every alternative, a group of alternatives multiplies them; duplicates and alternatives that contain another are dropped.

## Methods

- [AtTheEdge(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/methods.md#attheedge-isecuritycontext) : `ISecurityContext` `abstract` -- The caller as policies see them at the edge, where there is no request yet: reading the request throws.

## Fields

- [MaxAlternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/fields.md#maxalternatives) : `int` `static` -- The most alternatives that are published.
- [PolicyPrefix](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/fields.md#policyprefix) : `string` `static` -- What an edge policy name starts with; the rest is the plan's [RequestAuthorizationPlan.Id](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#id).
