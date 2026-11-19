# WktConverterFactory

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/Spatial/WktConverterFactory.cs)
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

- [WktConverterFactory](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor) -- Creates an instance of this class using the defaults.
- [WktConverterFactory(GeometryFactory)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor-geometryfactory) -- Creates an instance of this class using the provided `Geometries.GeometryFactory` and defaults for other values.
- [WktConverterFactory(GeometryFactory, bool)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor-geometryfactory-bool) -- Creates an instance of this class using the provided `Geometries.GeometryFactory`, the given value for whether or not we should write out a "bbox" for a plain geometry, and defaults for all other values.
- [WktConverterFactory(GeometryFactory, bool, string)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.spatial/wktconverterfactory/constructors.md#constructor-geometryfactory-bool-string) -- Creates an instance of this class using the provided `Geometries.GeometryFactory`, the given value for whether or not we should write out a "bbox" for a plain geometry, and the given "magic" string to signal when an `Features.IAttributesTable` property is actually filling in for a Feature's "id".

## Methods

- [CreateConverter(Type, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.spatial/wktconverterfactory/methods.md#createconverter-type-jsonserializeroptions) : `JsonConverter` -- Creates a converter for a specified type.
