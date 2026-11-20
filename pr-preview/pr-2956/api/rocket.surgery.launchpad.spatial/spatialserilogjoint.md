# SpatialSerilogJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/Spatial/Joints/SpatialSerilogJoint.cs)
- Implements: `IDovetailJoint`, `ISerilogJoint`

A joint that destructures the NetTopologySuite types in the logger Dovetail creates, the way [NetTopologySuiteLoggerConfigurationExtensions.NetTopologySuiteTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions/methods.md#nettopologysuitetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`

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

- [SpatialSerilogJoint](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.spatial/spatialserilogjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection, LoggerConfiguration)](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.spatial/spatialserilogjoint/methods.md#register-idovetailcontext-iservicecollection-loggerconfiguration) -- Register the Serilog part.

## See Also

- `Joints.ISerilogJoint`
