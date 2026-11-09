# CoordinatesSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net10.0`
- Inherits: `JsonElementScalarSerializer<Coordinate>`

A general purpose serializer for Coordinates

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

public class CoordinatesSerializer
    : Rocket.Surgery.LaunchPad.StrawberryShake.JsonElementScalarSerializer<NetTopologySuite.Geometries.Coordinate>
{
    // ...
}
```

## Constructors

- [CoordinatesSerializer(IOptions<JsonSerializerOptions>)](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer/constructors.md#constructor-ioptions-jsonserializeroptions) -- Create a new CoordinatesSerializer
