# JsonElementScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.strawberryshake.md)
- Type: [JsonElementScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.strawberryshake/jsonelementscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Base class for JsonElement Scalar Serializers

<a id="parse"></a>
<a id="parse-jsonelement"></a>

## Parse(JsonElement)

- Name: `Parse(JsonElement)`
- Modifiers: `override`
- Returns: `TRuntime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/StrawberryShake/JsonElementScalarSerializer.cs#L24)

Parses the serialized value to the runtime representation.

```csharp
public abstract class JsonElementScalarSerializer<TRuntime>
{
    public override TRuntime Parse(
        JsonElement serializedValue)
    {
        // ...
    }
}
```

## Parameters

- `serializedValue` (`JsonElement`)
  The serialized value.

## Returns

`TRuntime` -- Returns the parsed runtime representation.
