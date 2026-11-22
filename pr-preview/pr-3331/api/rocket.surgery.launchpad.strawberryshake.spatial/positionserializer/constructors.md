# PositionSerializer Constructors

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [PositionSerializer](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer.md)
- Kind: `Constructors`
- Members: `1`

Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

<a id="constructor"></a>

## PositionSerializer

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/StrawberryShake.Spatial/PositionSerializer.cs#L17)

Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

```csharp
public class PositionSerializer
{
    public PositionSerializer()
    {
        // ...
    }
}
```

## Remarks

The coordinates of the GeoJSON object types ( `GeoJSONPointType` and the rest) are positions, so a client that selects them needs this serializer: without it StrawberryShake reads the unknown scalar as a string and fails on the array.
