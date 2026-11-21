# NodaTimeStringScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [NodaTimeStringScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestringscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Shared base class for NodaTime string serializers

<a id="parse"></a>
<a id="parse-string"></a>

## Parse(string)

- Name: `Parse(string)`
- Modifiers: `override`
- Returns: `TRuntime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/NodaTime.StrawberryShake/NodaTimeStringScalarSerializer.cs#L17)

Parses the serialized value to the runtime representation.

```csharp
public abstract class NodaTimeStringScalarSerializer<TRuntime>
{
    public override TRuntime Parse(
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

`TRuntime` -- Returns the parsed runtime representation.
