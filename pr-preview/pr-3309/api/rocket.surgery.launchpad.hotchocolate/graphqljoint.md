# GraphqlJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1264`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/HotChocolate/Joints/GraphqlJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

The graph ql Joint

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

public class GraphqlJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Adds GraphQL with the LaunchPad error filter, and runs before the [HotChocolateJoint](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md).

## Constructors

- [GraphqlJoint](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
