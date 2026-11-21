# SpatialSerilogJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-pr.1283`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.d852442c68f31f012f495e373ebac8fda5751397/src/Spatial/Joints/SpatialSerilogJoint.cs)
- Implements: `IDovetailJoint`, `ISerilogJoint`

A joint that destructures the NetTopologySuite types in the logger Dovetail creates, the way [NetTopologySuiteLoggerConfigurationExtensions.NetTopologySuiteTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions/methods.md#nettopologysuitetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Spatial.Joints;

public class SpatialSerilogJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.ISerilogJoint
{
    // ...
}
```

## Constructors

- [SpatialSerilogJoint](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.spatial/spatialserilogjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection, LoggerConfiguration)](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.spatial/spatialserilogjoint/methods.md#register-idovetailcontext-iservicecollection-loggerconfiguration) -- Register the Serilog part.

## See Also

- `Joints.ISerilogJoint`
