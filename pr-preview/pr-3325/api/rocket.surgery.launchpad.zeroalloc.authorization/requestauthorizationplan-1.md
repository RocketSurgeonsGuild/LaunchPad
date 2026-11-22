# RequestAuthorizationPlan<TRequest>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1298`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)
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
