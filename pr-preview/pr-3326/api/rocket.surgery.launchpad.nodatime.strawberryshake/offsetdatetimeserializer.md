# OffsetDateTimeSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3326-merge.Sha.03f97a4ece74153c5b06eb64e3df4c70a1e53a62/src/NodaTime.StrawberryShake/OffsetDateTimeSerializer.cs)
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

- [OffsetDateTimeSerializer](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.nodatime.strawberryshake/offsetdatetimeserializer/constructors.md#constructor) -- Create a new OffsetDateTimeSerializer
