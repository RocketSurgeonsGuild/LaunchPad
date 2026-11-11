# DateTimeZoneSerializer Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [DateTimeZoneSerializer](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.nodatime.strawberryshake/datetimezoneserializer.md)
- Kind: `Methods`
- Members: `1`

General purpose DateTimeZone Serializer, which writes the zone id

<a id="parse"></a>
<a id="parse-string"></a>

## Parse(string)

- Name: `Parse(string)`
- Modifiers: `override`
- Returns: `DateTimeZone`

Parses the serialized value to the runtime representation.

```csharp
public class DateTimeZoneSerializer
{
    public override DateTimeZone Parse(
        string serializedValue)
    {
        // ...
    }
}
```

## Parameters

- `serializedValue` (`string`)
  The serialized value.

## Returns

`DateTimeZone` -- Returns the parsed runtime representation.
