# NetTopologySuiteSystemTextJsonSerializationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions.md)
- Kind: `Methods`
- Members: `2`

Extensions for NetTopologySuite

<a id="configuregeojsonforlaunchpad"></a>
<a id="configuregeojsonforlaunchpad-jsonserializeroptions-geometryfactory"></a>

## ConfigureGeoJsonForLaunchPad(JsonSerializerOptions, GeometryFactory?)

- Name: `ConfigureGeoJsonForLaunchPad(JsonSerializerOptions, GeometryFactory?)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs#L34-L43)

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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs#L54-L63)

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
