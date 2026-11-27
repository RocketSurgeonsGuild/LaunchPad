# OffsetDateTimeSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1299`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/NodaTime.StrawberryShake/OffsetDateTimeSerializer.cs)
- Inherits: `NodaTimeStringScalarSerializer<OffsetDateTime>`

Serializes `OffsetDateTime` as the `DateTime` scalar of HotChocolate 16

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

public class OffsetDateTimeSerializer
    : Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.NodaTimeStringScalarSerializer<NodaTime.OffsetDateTime>
{
    // ...
}
```

## Remarks

HotChocolate 16 renamed the `OffsetDateTime` scalar to `DateTime`.

## Constructors

- [OffsetDateTimeSerializer](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.nodatime.strawberryshake/offsetdatetimeserializer/constructors.md#constructor) -- Create a new OffsetDateTimeSerializer
