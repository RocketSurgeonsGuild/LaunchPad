# NetTopologySuiteSystemTextJsonSerializationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions.md)
- Kind: `Methods`
- Members: `2`

Extensions for NetTopologySuite

<a id="configuregeojsonforlaunchpad"></a>
<a id="configuregeojsonforlaunchpad-jsonserializeroptions-geometryfactory"></a>

## ConfigureGeoJsonForLaunchPad(JsonSerializerOptions, GeometryFactory?)

- Name: `ConfigureGeoJsonForLaunchPad(JsonSerializerOptions, GeometryFactory?)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs#L34-L43)

Configure System.Text.Json with defaults for launchpad

```csharp
public static class NetTopologySuiteSystemTextJsonSerializationExtensions
{
    public static JsonSerializerOptions ConfigureGeoJsonForLaunchPad(
        this JsonSerializerOptions options,
        GeometryFactory? factory)
    {
        // ...
    }
}
```

## Parameters

- `options` (`JsonSerializerOptions`)
- `factory` (`GeometryFactory?`)

<a id="configurewellknowntextforlaunchpad"></a>
<a id="configurewellknowntextforlaunchpad-jsonserializeroptions-geometryfactory"></a>

## ConfigureWellKnownTextForLaunchPad(JsonSerializerOptions, GeometryFactory?)

- Name: `ConfigureWellKnownTextForLaunchPad(JsonSerializerOptions, GeometryFactory?)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs#L54-L63)

Configure System.Text.Json with defaults for launchpad

```csharp
public static class NetTopologySuiteSystemTextJsonSerializationExtensions
{
    public static JsonSerializerOptions ConfigureWellKnownTextForLaunchPad(
        this JsonSerializerOptions options,
        GeometryFactory? factory)
    {
        // ...
    }
}
```

## Parameters

- `options` (`JsonSerializerOptions`)
- `factory` (`GeometryFactory?`)
