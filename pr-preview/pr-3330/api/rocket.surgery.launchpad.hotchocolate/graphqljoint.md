# GraphqlJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1282`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3330-merge.Sha.8f1e0adb726291eaf19241f8ed2aef23d09d52f6/src/HotChocolate/Joints/GraphqlJoint.cs)
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

Adds GraphQL with the LaunchPad error filter, and runs before the [HotChocolateJoint](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md).

## Constructors

- [GraphqlJoint](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
