# NodaTimeHotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs)
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

- [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
