# GeometrySerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3296-merge.Sha.e2170d7d4b36cc435a264495ab512cf086f53980/src/StrawberryShake.Spatial/GeometrySerializer.cs)
- Inherits: `JsonElementScalarSerializer<Geometry>`

General purpose Geometry Serializer

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

public class GeometrySerializer
    : Rocket.Surgery.LaunchPad.StrawberryShake.JsonElementScalarSerializer<NetTopologySuite.Geometries.Geometry>
{
    // ...
}
```

## Constructors

- [GeometrySerializer(IOptions<JsonSerializerOptions>)](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.strawberryshake.spatial/geometryserializer/constructors.md#constructor-ioptions-jsonserializeroptions) -- Construct a new GeometrySerializer
