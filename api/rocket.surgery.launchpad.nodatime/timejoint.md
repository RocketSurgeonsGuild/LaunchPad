# TimeJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-beta.325`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net10.0`
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
