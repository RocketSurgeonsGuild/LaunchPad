# NetTopologySuiteSystemTextJsonSerializationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-pr.1264`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs)

Extensions for NetTopologySuite

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Spatial;

public static class NetTopologySuiteSystemTextJsonSerializationExtensions
{
    // ...
}
```

## Remarks

Both methods also chain [NetTopologySuiteSystemTextJsonSerializationExtensions.TypeInfoResolver](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/properties.md#typeinforesolver) into the options' `JsonSerializerOptions.TypeInfoResolverChain`, so the geometry and feature types resolve in a trimmed or native AOT application. No reflection based resolver is added: options that should reflect over other types need one of their own.

## Properties

- [TypeInfoResolver](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/properties.md#typeinforesolver) : `IJsonTypeInfoResolver` `static` `get` -- Resolves the metadata of the geometry and feature types from the converter factory in the options, without reflection, for when they are serialized at the root or as an `Object`.

## Methods

- [ConfigureGeoJsonForLaunchPad(JsonSerializerOptions, GeometryFactory?)](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/methods.md#configuregeojsonforlaunchpad-jsonserializeroptions-geometryfactory) : `JsonSerializerOptions` `extension` -- Configure System.Text.Json with defaults for launchpad
- [ConfigureWellKnownTextForLaunchPad(JsonSerializerOptions, GeometryFactory?)](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/methods.md#configurewellknowntextforlaunchpad-jsonserializeroptions-geometryfactory) : `JsonSerializerOptions` `extension` -- Configure System.Text.Json with defaults for launchpad
