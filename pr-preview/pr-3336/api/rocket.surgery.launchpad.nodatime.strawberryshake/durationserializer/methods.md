# DurationSerializer Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [DurationSerializer](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.nodatime.strawberryshake/durationserializer.md)
- Kind: `Methods`
- Members: `1`

General purpose Duration Serializer

<a id="parse"></a>
<a id="parse-string"></a>

## Parse(string)

- Name: `Parse(string)`
- Modifiers: `override`
- Returns: `Duration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/NodaTime.StrawberryShake/DurationSerializer.cs#L28-L30)

Parses the serialized value to the runtime representation.

```csharp
public class DurationSerializer
{
    public override Duration Parse(
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

`Duration` -- Returns the parsed runtime representation.
