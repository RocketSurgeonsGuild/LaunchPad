# RequestAuthorizationPlan<TRequest>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)
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

## Constructors

- [RequestAuthorizationPlan(PolicyGroup[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1/constructors.md#constructor-policygroup) -- The plan of `TRequest`.

## Methods

- [AtTheEdge(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1/methods.md#attheedge-isecuritycontext) : `ISecurityContext` -- The caller as policies see them at the edge, where there is no request yet: reading the request throws.
- [WithRequest(ISecurityContext, TRequest)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1/methods.md#withrequest-isecuritycontext-trequest) : `ISecurityContext` -- The caller as policies see them in the pipeline: with the request as the resource.
