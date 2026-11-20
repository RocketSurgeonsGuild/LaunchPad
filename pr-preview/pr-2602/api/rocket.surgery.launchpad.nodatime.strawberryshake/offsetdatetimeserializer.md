# OffsetDateTimeSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/NodaTime.StrawberryShake/OffsetDateTimeSerializer.cs)
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

- [OffsetDateTimeSerializer](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime.strawberryshake/offsetdatetimeserializer/constructors.md#constructor) -- Create a new OffsetDateTimeSerializer
