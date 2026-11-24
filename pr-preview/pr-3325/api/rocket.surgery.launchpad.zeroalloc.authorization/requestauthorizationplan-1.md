# RequestAuthorizationPlan<TRequest>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)
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
