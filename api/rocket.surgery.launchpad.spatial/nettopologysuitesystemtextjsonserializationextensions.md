# NetTopologySuiteSystemTextJsonSerializationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/api/rocket.surgery.launchpad.spatial.md)
- Version: `2.0.1-beta.327`
- Namespace: `Rocket.Surgery.LaunchPad.Spatial`
- Target framework: `net10.0`

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

Both methods also chain [NetTopologySuiteSystemTextJsonSerializationExtensions.TypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/properties.md#typeinforesolver) into the options' `JsonSerializerOptions.TypeInfoResolverChain`, so the geometry and feature types resolve in a trimmed or native AOT application. No reflection based resolver is added: options that should reflect over other types need one of their own.

## Properties

- [TypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/properties.md#typeinforesolver) : `IJsonTypeInfoResolver` `static` `get` -- Resolves the metadata of the geometry and feature types from the converter factory in the options, without reflection, for when they are serialized at the root or as an `Object`.

## Methods

- [ConfigureGeoJsonForLaunchPad(JsonSerializerOptions, GeometryFactory?)](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/methods.md#configuregeojsonforlaunchpad-jsonserializeroptions-geometryfactory) : `JsonSerializerOptions` `extension` -- Configure System.Text.Json with defaults for launchpad
- [ConfigureWellKnownTextForLaunchPad(JsonSerializerOptions, GeometryFactory?)](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions/methods.md#configurewellknowntextforlaunchpad-jsonserializeroptions-geometryfactory) : `JsonSerializerOptions` `extension` -- Configure System.Text.Json with defaults for launchpad
