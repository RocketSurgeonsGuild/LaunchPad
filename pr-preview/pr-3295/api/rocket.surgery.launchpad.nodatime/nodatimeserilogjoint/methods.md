# NodaTimeSerilogJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSerilogJoint](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.nodatime/nodatimeserilogjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that destructures the NodaTime types in the logger Dovetail creates, the way [NodaTimeLoggerConfigurationExtensions.NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection-loggerconfiguration"></a>

## Register(IDovetailContext, IServiceCollection, LoggerConfiguration)

- Name: `Register(IDovetailContext, IServiceCollection, LoggerConfiguration)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3295-merge.Sha.8032622be33212edca749d51eeb2f284c76d51b7/src/NodaTime/Joints/NodaTimeSerilogJoint.cs#L21-L24)

Register the Serilog part.

```csharp
public class NodaTimeSerilogJoint
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
