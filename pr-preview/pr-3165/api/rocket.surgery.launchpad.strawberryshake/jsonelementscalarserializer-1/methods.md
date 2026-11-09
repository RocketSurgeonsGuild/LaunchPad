# JsonElementScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.strawberryshake.md)
- Type: [JsonElementScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.strawberryshake/jsonelementscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Base class for JsonElement Scalar Serializers

<a id="parse"></a>
<a id="parse-jsonelement"></a>

## Parse(JsonElement)

- Name: `Parse(JsonElement)`
- Modifiers: `override`
- Returns: `TRuntime`

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
