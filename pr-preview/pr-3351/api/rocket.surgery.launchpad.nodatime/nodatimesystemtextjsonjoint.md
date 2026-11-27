# NodaTimeSystemTextJsonJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1305`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/NodaTime/Joints/NodaTimeSystemTextJsonJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that configures `Json.JsonSerializerOptions` with the NodaTime converters, using the `IDateTimeZoneProvider` that is registered. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.Joints;

public class NodaTimeSystemTextJsonJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The time zone provider is resolved when the options are first built, so it does not matter whether it is registered before or after this joint. The [TimeJoint](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.nodatime/timejoint.md) registers the provider an application uses by default.

## Constructors

- [NodaTimeSystemTextJsonJoint](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
