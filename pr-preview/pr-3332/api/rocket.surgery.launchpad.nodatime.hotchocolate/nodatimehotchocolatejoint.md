# NodaTimeHotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs)
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

- [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
