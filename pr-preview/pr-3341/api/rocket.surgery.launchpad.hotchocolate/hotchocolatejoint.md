# HotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1288`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/HotChocolate/Joints/HotChocolateJoint.cs)
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

- [HotChocolateJoint](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
