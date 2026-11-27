# PositionSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1292`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/StrawberryShake.Spatial/PositionSerializer.cs)
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

- [PositionSerializer](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer/constructors.md#constructor) -- Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

## Methods

- [Parse(JsonElement)](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer/methods.md#parse-jsonelement) : `Coordinate` -- Parses the serialized value to the runtime representation.
