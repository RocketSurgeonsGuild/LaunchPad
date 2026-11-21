# HotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1272`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/HotChocolate/Joints/HotChocolateJoint.cs)
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

- [HotChocolateJoint](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
