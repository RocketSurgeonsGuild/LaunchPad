# GeometrySerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/StrawberryShake.Spatial/GeometrySerializer.cs)
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

- [GeometrySerializer(IOptions<JsonSerializerOptions>)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.strawberryshake.spatial/geometryserializer/constructors.md#constructor-ioptions-jsonserializeroptions) -- Construct a new GeometrySerializer
