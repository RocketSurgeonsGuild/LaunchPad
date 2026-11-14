# WktConverterFactory Constructors

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.spatial.md)
- Type: [WktConverterFactory](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.spatial/wktconverterfactory.md)
- Kind: `Constructors`
- Members: `4`

A converter for

<a id="constructor"></a>

## WktConverterFactory

- Name: `Constructor`

Creates an instance of this class using the defaults.

```csharp
public class WktConverterFactory
{
    public WktConverterFactory()
    {
        // ...
    }
}
```

<a id="constructor-geometryfactory"></a>

## WktConverterFactory(GeometryFactory)

- Name: `Constructor(GeometryFactory)`

Creates an instance of this class using the provided `Geometries.GeometryFactory` and defaults for other values.

```csharp
public class WktConverterFactory
{
    public WktConverterFactory(
        GeometryFactory factory)
    {
        // ...
    }
}
```

## Parameters

- `factory` (`GeometryFactory`)

<a id="constructor-geometryfactory-bool"></a>

## WktConverterFactory(GeometryFactory, bool)

- Name: `Constructor(GeometryFactory, bool)`

Creates an instance of this class using the provided `Geometries.GeometryFactory`, the given value for whether or not we should write out a "bbox" for a plain geometry, and defaults for all other values.

```csharp
public class WktConverterFactory
{
    public WktConverterFactory(
        GeometryFactory factory,
        bool writeGeometryBBox)
    {
        // ...
    }
}
```

## Parameters

- `factory` (`GeometryFactory`)
- `writeGeometryBBox` (`bool`)

<a id="constructor-geometryfactory-bool-string"></a>

## WktConverterFactory(GeometryFactory, bool, string)

- Name: `Constructor(GeometryFactory, bool, string)`

Creates an instance of this class using the provided `Geometries.GeometryFactory`, the given value for whether or not we should write out a "bbox" for a plain geometry, and the given "magic" string to signal when an `Features.IAttributesTable` property is actually filling in for a Feature's "id".

```csharp
public class WktConverterFactory
{
    public WktConverterFactory(
        GeometryFactory factory,
        bool writeGeometryBBox,
        string idPropertyName)
    {
        // ...
    }
}
```

## Parameters

- `factory` (`GeometryFactory`)
- `writeGeometryBBox` (`bool`)
- `idPropertyName` (`string`)
