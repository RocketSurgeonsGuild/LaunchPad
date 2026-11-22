# GraphqlJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/HotChocolate/Joints/GraphqlJoint.cs)
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

Adds GraphQL with the LaunchPad error filter, and runs before the [HotChocolateJoint](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md).

## Constructors

- [GraphqlJoint](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
