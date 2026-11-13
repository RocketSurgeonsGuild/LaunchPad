# Rocket.Surgery.LaunchPad.Spatial

- Package: `Rocket.Surgery.LaunchPad.Spatial`
- Version: `2.0.1-pr.1182`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.Spatial](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Spatial)

## Namespaces

### Rocket.Surgery.LaunchPad.Spatial

- [NetTopologySuiteLoggerConfigurationExtensions](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions.md) -- `class` `static` -- Adds the Destructure.NetTopologySuiteTypes() extension to `LoggerConfiguration`.
- [NetTopologySuiteSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions.md) -- `class` `static` -- Extensions for NetTopologySuite
- [WktConverterFactory](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.spatial/wktconverterfactory.md) -- `class` -- A converter for

### Rocket.Surgery.LaunchPad.Spatial.Joints

- [SpatialJoint](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.spatial/spatialjoint.md) -- `class` -- Adds support for spatial types into System.Text.Json
- [SpatialSerilogJoint](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.spatial/spatialserilogjoint.md) -- `class` -- A joint that destructures the NetTopologySuite types in the logger Dovetail creates, the way [NetTopologySuiteLoggerConfigurationExtensions.NetTopologySuiteTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions/methods.md#nettopologysuitetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`
