# WktConverterFactory Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial.md)
- Type: [WktConverterFactory](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial/wktconverterfactory.md)
- Kind: `Methods`
- Members: `1`

A converter for

<a id="createconverter"></a>
<a id="createconverter-type-jsonserializeroptions"></a>

## CreateConverter(Type, JsonSerializerOptions)

- Name: `CreateConverter(Type, JsonSerializerOptions)`
- Modifiers: `override`
- Returns: `JsonConverter`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.28f891b4cf6958b8ac5addf5a93485615fecf518/src/Spatial/WktConverterFactory.cs#L77-L80)

Creates a converter for a specified type.

```csharp
public class WktConverterFactory
{
    public override JsonConverter CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `typeToConvert` (`Type`)
  The type handled by the converter.
- `options` (`JsonSerializerOptions`)
  The serialization options to use.

## Returns

`JsonConverter` -- A converter for which `T` is compatible with `typeToConvert`.
