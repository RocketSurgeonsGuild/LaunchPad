# CoordinatesSerializer Constructors

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [CoordinatesSerializer](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer.md)
- Kind: `Constructors`
- Members: `1`

Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

<a id="constructor"></a>

## CoordinatesSerializer

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/StrawberryShake.Spatial/CoordinatesSerializer.cs#L15)

Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

```csharp
public class CoordinatesSerializer
{
    public CoordinatesSerializer()
    {
        // ...
    }
}
```

## Remarks

The depth depends on the geometry, so there is no one runtime type for it: the value is handed over as the JSON it was sent as.
