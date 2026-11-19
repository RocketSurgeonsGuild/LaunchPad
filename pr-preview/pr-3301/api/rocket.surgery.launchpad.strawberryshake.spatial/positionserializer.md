# PositionSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/StrawberryShake.Spatial/PositionSerializer.cs)
- Inherits: `ScalarSerializer<JsonElement, Coordinate>`

Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

public class PositionSerializer
    : StrawberryShake.Serialization.ScalarSerializer<System.Text.Json.JsonElement, NetTopologySuite.Geometries.Coordinate>
{
    // ...
}
```

## Remarks

The coordinates of the GeoJSON object types ( `GeoJSONPointType` and the rest) are positions, so a client that selects them needs this serializer: without it StrawberryShake reads the unknown scalar as a string and fails on the array.

## Constructors

- [PositionSerializer](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer/constructors.md#constructor) -- Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

## Methods

- [Parse(JsonElement)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer/methods.md#parse-jsonelement) : `Coordinate` -- Parses the serialized value to the runtime representation.
