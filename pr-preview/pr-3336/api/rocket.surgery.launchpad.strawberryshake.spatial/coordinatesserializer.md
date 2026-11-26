# CoordinatesSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1292`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.9ec91d866bcac66448bc0f439b45a4702f1b73d3/src/StrawberryShake.Spatial/CoordinatesSerializer.cs)
- Inherits: `ScalarSerializer<JsonElement, JsonElement>`

Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

public class CoordinatesSerializer
    : StrawberryShake.Serialization.ScalarSerializer<System.Text.Json.JsonElement, System.Text.Json.JsonElement>
{
    // ...
}
```

## Remarks

The depth depends on the geometry, so there is no one runtime type for it: the value is handed over as the JSON it was sent as.

## Constructors

- [CoordinatesSerializer](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer/constructors.md#constructor) -- Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

## Methods

- [Parse(JsonElement)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer/methods.md#parse-jsonelement) : `JsonElement` -- Parses the serialized value to the runtime representation.
