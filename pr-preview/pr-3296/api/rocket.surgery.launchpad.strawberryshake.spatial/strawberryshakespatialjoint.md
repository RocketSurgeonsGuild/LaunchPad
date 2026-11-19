# StrawberryShakeSpatialJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3296-merge.Sha.e2170d7d4b36cc435a264495ab512cf086f53980/src/StrawberryShake.Spatial/Joints/StrawberryShakeSpatialJoint.cs)
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

- [StrawberryShakeSpatialJoint](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
