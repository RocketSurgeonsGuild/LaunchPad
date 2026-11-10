# NodaTimeStrawberryShakeJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1163`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the NodaTime serializers to StrawberryShake

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints;

public class NodaTimeStrawberryShakeJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Every NodaTime scalar that `AddNodaTime` puts in a HotChocolate schema has a serializer here, and all of them are registered, [OffsetDateSerializer](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake/offsetdateserializer.md) and [DateTimeZoneSerializer](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake/datetimezoneserializer.md) included (decided in RocketSurgeonsGuild/LaunchPad#3066). A generated client resolves a serializer for every scalar its operations use, and cannot create its serializer resolver, for any operation, when one of them is missing.

[ZonedDateTimeSerializer](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake/zoneddatetimeserializer.md) and [DateTimeZoneSerializer](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake/datetimezoneserializer.md) resolve the `IDateTimeZoneProvider` that the `TimeJoint` of Rocket.Surgery.LaunchPad.NodaTime registers.

## Constructors

- [NodaTimeStrawberryShakeJoint](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestrawberryshakejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestrawberryshakejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
