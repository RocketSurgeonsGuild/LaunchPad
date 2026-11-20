# NodaTimeSerilogJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/NodaTime/Joints/NodaTimeSerilogJoint.cs)
- Implements: `IDovetailJoint`, `ISerilogJoint`

A joint that destructures the NodaTime types in the logger Dovetail creates, the way [NodaTimeLoggerConfigurationExtensions.NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.Joints;

public class NodaTimeSerilogJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.ISerilogJoint
{
    // ...
}
```

## Constructors

- [NodaTimeSerilogJoint](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.nodatime/nodatimeserilogjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection, LoggerConfiguration)](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.nodatime/nodatimeserilogjoint/methods.md#register-idovetailcontext-iservicecollection-loggerconfiguration) -- Register the Serilog part.

## See Also

- `Joints.ISerilogJoint`
