# HotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-beta.448`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.974bfd9a73f64517e8a81d86b29393f3cce75778/src/HotChocolate/Joints/HotChocolateJoint.cs)
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

- [HotChocolateJoint](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
