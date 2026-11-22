# TimeJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-beta.447`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.afac89ac2f84551e9433e9fb435d06d6d3ff37f0/src/NodaTime/Joints/TimeJoint.cs)
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

Dovetail resolves the [NodaTimeOptions](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md) from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [TimeJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime/timejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/timejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
