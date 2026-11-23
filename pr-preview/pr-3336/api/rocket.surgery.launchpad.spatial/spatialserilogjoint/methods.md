# SpatialSerilogJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial.md)
- Type: [SpatialSerilogJoint](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial/spatialserilogjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that destructures the NetTopologySuite types in the logger Dovetail creates, the way [NetTopologySuiteLoggerConfigurationExtensions.NetTopologySuiteTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions/methods.md#nettopologysuitetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection-loggerconfiguration"></a>

## Register(IDovetailContext, IServiceCollection, LoggerConfiguration)

- Name: `Register(IDovetailContext, IServiceCollection, LoggerConfiguration)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.28f891b4cf6958b8ac5addf5a93485615fecf518/src/Spatial/Joints/SpatialSerilogJoint.cs#L21-L24)

Register the Serilog part.

```csharp
public class SpatialSerilogJoint
{
    public void Register(
        IDovetailContext context,
        IServiceCollection services,
        LoggerConfiguration loggerConfiguration)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IDovetailContext`)
- `services` (`IServiceCollection`)
- `loggerConfiguration` (`LoggerConfiguration`)
