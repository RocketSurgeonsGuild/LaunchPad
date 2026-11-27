# GeometrySerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-beta.462`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.cd7db14e49b12478b13b2f21edf214c613078b4c/src/StrawberryShake.Spatial/GeometrySerializer.cs)
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

- [GeometrySerializer(IOptions<JsonSerializerOptions>)](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/geometryserializer/constructors.md#constructor-ioptions-jsonserializeroptions) -- Construct a new GeometrySerializer
