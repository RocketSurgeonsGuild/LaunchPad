# GraphqlJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/HotChocolate/Joints/GraphqlJoint.cs)
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

Adds GraphQL with the LaunchPad error filter, and runs before the [HotChocolateJoint](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md).

## Constructors

- [GraphqlJoint](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate/graphqljoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
