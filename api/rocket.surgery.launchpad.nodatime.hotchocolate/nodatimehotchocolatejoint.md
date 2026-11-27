# NodaTimeHotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-beta.459`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.8446cfb49f81338ce4d090fef0bb3afa0711fa3f/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the NodaTime types to the HotChocolate schema

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints;

public class NodaTimeHotChocolateJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The scalars are HotChocolate's own ( `AddNodaTime()` from HotChocolate.Types.NodaTime): `DateTime` for `OffsetDateTime`, `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`. HotChocolate 16 has no scalar for `Instant`, so the Joint binds it to the `DateTime` scalar as an `OffsetDateTime` at UTC, which loses nothing. The Joint binds the same types to comparable filters. The other NodaTime types (ZonedDateTime, OffsetDate, OffsetTime, Offset, Period, DateTimeZone, IsoDayOfWeek) have no scalar.

## Constructors

- [NodaTimeHotChocolateJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
