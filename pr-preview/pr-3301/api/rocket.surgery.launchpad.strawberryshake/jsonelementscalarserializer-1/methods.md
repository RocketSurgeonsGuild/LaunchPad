# JsonElementScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.strawberryshake.md)
- Type: [JsonElementScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.strawberryshake/jsonelementscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Base class for JsonElement Scalar Serializers

<a id="parse"></a>
<a id="parse-jsonelement"></a>

## Parse(JsonElement)

- Name: `Parse(JsonElement)`
- Modifiers: `override`
- Returns: `TRuntime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/StrawberryShake/JsonElementScalarSerializer.cs#L24)

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
