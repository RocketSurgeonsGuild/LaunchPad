# DurationSerializer Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3294/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [DurationSerializer](/LaunchPad/pr-preview/pr-3294/api/rocket.surgery.launchpad.nodatime.strawberryshake/durationserializer.md)
- Kind: `Methods`
- Members: `1`

General purpose Duration Serializer

<a id="parse"></a>
<a id="parse-string"></a>

## Parse(string)

- Name: `Parse(string)`
- Modifiers: `override`
- Returns: `Duration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3294-merge.Sha.8ed83894fba92c2ba970626558f3059478959109/src/NodaTime.StrawberryShake/DurationSerializer.cs#L28-L30)

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
