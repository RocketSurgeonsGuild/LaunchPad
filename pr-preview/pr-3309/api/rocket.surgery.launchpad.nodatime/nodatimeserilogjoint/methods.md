# NodaTimeSerilogJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSerilogJoint](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.nodatime/nodatimeserilogjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that destructures the NodaTime types in the logger Dovetail creates, the way [NodaTimeLoggerConfigurationExtensions.NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection-loggerconfiguration"></a>

## Register(IDovetailContext, IServiceCollection, LoggerConfiguration)

- Name: `Register(IDovetailContext, IServiceCollection, LoggerConfiguration)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/NodaTime/Joints/NodaTimeSerilogJoint.cs#L21-L24)

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
