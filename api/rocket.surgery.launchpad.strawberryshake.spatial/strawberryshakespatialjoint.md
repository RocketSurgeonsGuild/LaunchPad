# StrawberryShakeSpatialJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-beta.447`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.afac89ac2f84551e9433e9fb435d06d6d3ff37f0/src/StrawberryShake.Spatial/Joints/StrawberryShakeSpatialJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the spatial serializers to StrawberryShake

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints;

public class StrawberryShakeSpatialJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [StrawberryShakeSpatialJoint](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
