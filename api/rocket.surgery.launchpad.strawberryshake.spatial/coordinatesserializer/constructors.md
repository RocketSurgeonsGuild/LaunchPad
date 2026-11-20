# CoordinatesSerializer Constructors

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [CoordinatesSerializer](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer.md)
- Kind: `Constructors`
- Members: `1`

Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

<a id="constructor"></a>

## CoordinatesSerializer

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6f19e0829a4004d537d23cc908a809ddcba9219c/src/StrawberryShake.Spatial/CoordinatesSerializer.cs#L15)

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
