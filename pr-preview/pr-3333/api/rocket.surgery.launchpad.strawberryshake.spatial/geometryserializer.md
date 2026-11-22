# GeometrySerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3333-merge.Sha.836b758fb89040b6cc85ed7817191956e959bc1f/src/StrawberryShake.Spatial/GeometrySerializer.cs)
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

- [GeometrySerializer(IOptions<JsonSerializerOptions>)](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.strawberryshake.spatial/geometryserializer/constructors.md#constructor-ioptions-jsonserializeroptions) -- Construct a new GeometrySerializer
