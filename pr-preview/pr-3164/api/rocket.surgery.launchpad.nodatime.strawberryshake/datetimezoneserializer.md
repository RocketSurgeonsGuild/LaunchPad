# DateTimeZoneSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1172`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net10.0`
- Inherits: `ScalarSerializer<string, DateTimeZone>`

General purpose DateTimeZone Serializer, which writes the zone id

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

public class DateTimeZoneSerializer
    : StrawberryShake.Serialization.ScalarSerializer<string, NodaTime.DateTimeZone>
{
    // ...
}
```

## Constructors

- [DateTimeZoneSerializer(IDateTimeZoneProvider)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.nodatime.strawberryshake/datetimezoneserializer/constructors.md#constructor-idatetimezoneprovider) -- General purpose DateTimeZone Serializer, which writes the zone id

## Methods

- [Parse(string)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.nodatime.strawberryshake/datetimezoneserializer/methods.md#parse-string) : `DateTimeZone` -- Parses the serialized value to the runtime representation.
