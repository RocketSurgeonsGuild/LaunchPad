# NodaTimeStringScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [NodaTimeStringScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestringscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Shared base class for NodaTime string serializers

<a id="parse"></a>
<a id="parse-string"></a>

## Parse(string)

- Name: `Parse(string)`
- Modifiers: `override`
- Returns: `TRuntime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/NodaTime.StrawberryShake/NodaTimeStringScalarSerializer.cs#L17)

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
