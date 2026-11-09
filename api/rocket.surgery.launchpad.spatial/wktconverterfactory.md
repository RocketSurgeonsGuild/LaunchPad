# WktConverterFactory

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-beta.322`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial`
- Target framework: `net10.0`
- Inherits: `GeoJsonConverterFactory`

A converter for

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Spatial;

public class WktConverterFactory
    : NetTopologySuite.IO.Converters.GeoJsonConverterFactory
{
    // ...
}
```

## Constructors

- [WktConverterFactory](/LaunchPad/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor) -- Creates an instance of this class using the defaults.
- [WktConverterFactory(GeometryFactory)](/LaunchPad/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor-geometryfactory) -- Creates an instance of this class using the provided `Geometries.GeometryFactory` and defaults for other values.
- [WktConverterFactory(GeometryFactory, bool)](/LaunchPad/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor-geometryfactory-bool) -- Creates an instance of this class using the provided `Geometries.GeometryFactory`, the given value for whether or not we should write out a "bbox" for a plain geometry, and defaults for all other values.
- [WktConverterFactory(GeometryFactory, bool, string)](/LaunchPad/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor-geometryfactory-bool-string) -- Creates an instance of this class using the provided `Geometries.GeometryFactory`, the given value for whether or not we should write out a "bbox" for a plain geometry, and the given "magic" string to signal when an `Features.IAttributesTable` property is actually filling in for a Feature's "id".

## Methods

- [CreateConverter(Type, JsonSerializerOptions)](/LaunchPad/api/rocket.surgery.launchpad.spatial/wktconverterfactory/methods.md#createconverter-type-jsonserializeroptions) : `JsonConverter` -- Creates a converter for a specified type.
