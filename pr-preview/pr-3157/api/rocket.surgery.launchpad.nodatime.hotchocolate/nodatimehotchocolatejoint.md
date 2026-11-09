# NodaTimeHotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1158`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints`
- Target framework: `net10.0`
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

The ZonedDateTime type parses with the `IDateTimeZoneProvider` instance registered in the services (the one the `Joints.TimeJoint` registers, so this Joint runs after it), or with a cache over `NodaTimeOptions.DateTimeZoneSource` when there is none.

## Constructors

- [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
