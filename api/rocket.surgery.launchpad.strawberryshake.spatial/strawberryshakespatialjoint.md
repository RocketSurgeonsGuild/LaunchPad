# StrawberryShakeSpatialJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-beta.435`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6aecd30ca14bc14267856dbc142a55a566b9ae42/src/StrawberryShake.Spatial/Joints/StrawberryShakeSpatialJoint.cs)
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
