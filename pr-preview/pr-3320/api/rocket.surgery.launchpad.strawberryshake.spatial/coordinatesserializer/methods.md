# CoordinatesSerializer Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [CoordinatesSerializer](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer.md)
- Kind: `Methods`
- Members: `1`

Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

<a id="parse"></a>
<a id="parse-jsonelement"></a>

## Parse(JsonElement)

- Name: `Parse(JsonElement)`
- Modifiers: `override`
- Returns: `JsonElement`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/StrawberryShake.Spatial/CoordinatesSerializer.cs#L18)

Parses the serialized value to the runtime representation.

```csharp
public class CoordinatesSerializer
{
    public override JsonElement Parse(
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

`JsonElement` -- Returns the parsed runtime representation.
