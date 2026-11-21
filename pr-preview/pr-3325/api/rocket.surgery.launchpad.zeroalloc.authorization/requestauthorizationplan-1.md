# RequestAuthorizationPlan<TRequest>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)
- Inherits: [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)

The plan of `TRequest`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestAuthorizationPlan<TRequest>
    : Rocket.Surgery.LaunchPad.Authorization.RequestAuthorizationPlan
{
    // ...
}
```

## Type Parameters

- `TRequest` -- The request.

## Properties

- [Instance](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1/properties.md#instance) : [RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md) `static` `get` -- The plan, or null for a request without policies.

## Methods

- [AtTheEdge(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1/methods.md#attheedge-isecuritycontext) : `ISecurityContext` -- The caller as policies see them at the edge, where there is no request yet: reading the request throws.
- [WithRequest(ISecurityContext, TRequest)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1/methods.md#withrequest-isecuritycontext-trequest) : `ISecurityContext` -- The caller as policies see them in the pipeline: with the request as the resource.
