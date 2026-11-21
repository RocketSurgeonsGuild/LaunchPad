# SpatialJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-beta.434`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.dfbe8776933590a8ca1efba66f9af5c4929d4975/src/Spatial/Joints/SpatialJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds support for spatial types into System.Text.Json

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Spatial.Joints;

public class SpatialJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

A service Joint that registers the geometry services and configures the System.Text.Json options. The NetTopologySuite types are destructured in Serilog by the [SpatialSerilogJoint](/LaunchPad/api/rocket.surgery.launchpad.spatial/spatialserilogjoint.md) of this package.

## Constructors

- [SpatialJoint](/LaunchPad/api/rocket.surgery.launchpad.spatial/spatialjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.spatial/spatialjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
