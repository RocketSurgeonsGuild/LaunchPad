# CoordinatesSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3238/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Version: `2.0.1-pr.1212`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Target framework: `net10.0`
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

- [CoordinatesSerializer](/LaunchPad/pr-preview/pr-3238/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer/constructors.md#constructor) -- Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)

## Methods

- [Parse(JsonElement)](/LaunchPad/pr-preview/pr-3238/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer/methods.md#parse-jsonelement) : `JsonElement` -- Parses the serialized value to the runtime representation.
