# PositionSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/StrawberryShake.Spatial/PositionSerializer.cs)
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

- [PositionSerializer](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer/constructors.md#constructor) -- Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

## Methods

- [Parse(JsonElement)](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer/methods.md#parse-jsonelement) : `Coordinate` -- Parses the serialized value to the runtime representation.
