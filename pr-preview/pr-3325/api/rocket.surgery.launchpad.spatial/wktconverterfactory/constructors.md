# WktConverterFactory Constructors

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.spatial.md)
- Type: [WktConverterFactory](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.spatial/wktconverterfactory.md)
- Kind: `Constructors`
- Members: `4`

A converter for

<a id="constructor"></a>

## WktConverterFactory

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/Spatial/WktConverterFactory.cs#L33-L35)

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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/Spatial/WktConverterFactory.cs#L43-L45)

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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/Spatial/WktConverterFactory.cs#L55-L57)

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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/Spatial/WktConverterFactory.cs#L69-L72)

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
