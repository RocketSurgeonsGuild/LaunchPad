# OffsetDateTimeSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-beta.423`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/NodaTime.StrawberryShake/OffsetDateTimeSerializer.cs)
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

- [OffsetDateTimeSerializer](/LaunchPad/api/rocket.surgery.launchpad.nodatime.strawberryshake/offsetdatetimeserializer/constructors.md#constructor) -- Create a new OffsetDateTimeSerializer
