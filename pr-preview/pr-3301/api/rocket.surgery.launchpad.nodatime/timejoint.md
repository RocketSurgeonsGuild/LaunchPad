# TimeJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/NodaTime/Joints/TimeJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`, `ISetupJoint`

A joint that registers the clock, time provider and time zone provider. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.Joints;

public class TimeJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint,
      Joinery.Dovetail.Joints.ISetupJoint
{
    // ...
}
```

## Remarks

Dovetail resolves the [NodaTimeOptions](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md) from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [TimeJoint](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.nodatime/timejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.nodatime/timejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
