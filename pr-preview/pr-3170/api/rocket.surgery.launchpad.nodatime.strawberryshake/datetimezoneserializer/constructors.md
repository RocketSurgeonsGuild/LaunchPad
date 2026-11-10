# DateTimeZoneSerializer Constructors

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [DateTimeZoneSerializer](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.nodatime.strawberryshake/datetimezoneserializer.md)
- Kind: `Constructors`
- Members: `1`

General purpose DateTimeZone Serializer, which writes the zone id

<a id="constructor"></a>
<a id="constructor-idatetimezoneprovider"></a>

## DateTimeZoneSerializer(IDateTimeZoneProvider)

- Name: `Constructor(IDateTimeZoneProvider)`

General purpose DateTimeZone Serializer, which writes the zone id

```csharp
public class DateTimeZoneSerializer
{
    public DateTimeZoneSerializer(
        IDateTimeZoneProvider provider)
    {
        // ...
    }
}
```

## Parameters

- `provider` (`IDateTimeZoneProvider`)
  The provider that resolves a zone id to its zone.
