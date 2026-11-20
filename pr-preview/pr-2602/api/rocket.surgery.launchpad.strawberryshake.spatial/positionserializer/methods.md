# PositionSerializer Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [PositionSerializer](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer.md)
- Kind: `Methods`
- Members: `1`

Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

<a id="parse"></a>
<a id="parse-jsonelement"></a>

## Parse(JsonElement)

- Name: `Parse(JsonElement)`
- Modifiers: `override`
- Returns: `Coordinate`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/StrawberryShake.Spatial/PositionSerializer.cs#L22-L29)

Parses the serialized value to the runtime representation.

```csharp
public class PositionSerializer
{
    public override Coordinate Parse(
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

`Coordinate` -- Returns the parsed runtime representation.
