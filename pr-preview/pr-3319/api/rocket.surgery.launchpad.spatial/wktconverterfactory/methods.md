# WktConverterFactory Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.spatial.md)
- Type: [WktConverterFactory](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.spatial/wktconverterfactory.md)
- Kind: `Methods`
- Members: `1`

A converter for

<a id="createconverter"></a>
<a id="createconverter-type-jsonserializeroptions"></a>

## CreateConverter(Type, JsonSerializerOptions)

- Name: `CreateConverter(Type, JsonSerializerOptions)`
- Modifiers: `override`
- Returns: `JsonConverter`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/Spatial/WktConverterFactory.cs#L77-L80)

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
