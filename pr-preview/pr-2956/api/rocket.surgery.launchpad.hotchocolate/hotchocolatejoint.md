# HotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/HotChocolate/Joints/HotChocolateJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Hot Chocolate Joint

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

public class HotChocolateJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The NodaTime types come from the `NodaTimeHotChocolateJoint` in Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.

## Constructors

- [HotChocolateJoint](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
