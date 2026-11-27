# ZeroAllocHotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/ZeroAlloc.HotChocolate/Joints/ZeroAllocHotChocolateJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Reports each failure of a request the ZeroAlloc mediator rejected as a GraphQL `VALIDATION` error. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

public class ZeroAllocHotChocolateJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Adds the [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md) to the GraphQL builder the other GraphQL joints share. Runs after the `Joints.GraphqlJoint`, so the filter comes after LaunchPad's error filter, which rebuilds every error from its exception and would collapse the one error per failure this filter reports.

## Constructors

- [ZeroAllocHotChocolateJoint](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
