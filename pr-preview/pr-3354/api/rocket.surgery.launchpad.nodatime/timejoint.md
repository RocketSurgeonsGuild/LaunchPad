# TimeJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/NodaTime/Joints/TimeJoint.cs)
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

Dovetail resolves the [NodaTimeOptions](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md) from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [TimeJoint](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.nodatime/timejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.nodatime/timejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
