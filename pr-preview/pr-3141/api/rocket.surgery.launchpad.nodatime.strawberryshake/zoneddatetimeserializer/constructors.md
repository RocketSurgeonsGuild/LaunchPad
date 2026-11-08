# ZonedDateTimeSerializer Constructors

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [ZonedDateTimeSerializer](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.nodatime.strawberryshake/zoneddatetimeserializer.md)
- Kind: `Constructors`
- Members: `1`

General purpose ZonedDateTime Serializer

<a id="constructor"></a>
<a id="constructor-idatetimezoneprovider"></a>

## ZonedDateTimeSerializer(IDateTimeZoneProvider)

- Name: `Constructor(IDateTimeZoneProvider)`

General purpose ZonedDateTime Serializer

```csharp
public class ZonedDateTimeSerializer
{
    public ZonedDateTimeSerializer(
        IDateTimeZoneProvider provider)
    {
        // ...
    }
}
```

## Parameters

- `provider` (`IDateTimeZoneProvider`)
  The provider that resolves the zone id in the serialized value.
