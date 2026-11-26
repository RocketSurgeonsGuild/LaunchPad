# Rocket.Surgery.LaunchPad.Spatial

- Package: `Rocket.Surgery.LaunchPad.Spatial`
- Version: `2.0.1-beta.456`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.main.Sha.0c205f7c0f7c99d017c3755b484180de4bcb850a/src/Spatial)
- NuGet: [Rocket.Surgery.LaunchPad.Spatial](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Spatial)

## Namespaces

### Rocket.Surgery.LaunchPad.Spatial

- [NetTopologySuiteLoggerConfigurationExtensions](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions.md) -- `class` `static` -- Adds the Destructure.NetTopologySuiteTypes() extension to `LoggerConfiguration`.
- [NetTopologySuiteSystemTextJsonSerializationExtensions](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions.md) -- `class` `static` -- Extensions for NetTopologySuite
- [WktConverterFactory](/LaunchPad/api/rocket.surgery.launchpad.spatial/wktconverterfactory.md) -- `class` -- A converter for

### Rocket.Surgery.LaunchPad.Spatial.Joints

- [SpatialJoint](/LaunchPad/api/rocket.surgery.launchpad.spatial/spatialjoint.md) -- `class` -- Adds support for spatial types into System.Text.Json
- [SpatialSerilogJoint](/LaunchPad/api/rocket.surgery.launchpad.spatial/spatialserilogjoint.md) -- `class` -- A joint that destructures the NetTopologySuite types in the logger Dovetail creates, the way [NetTopologySuiteLoggerConfigurationExtensions.NetTopologySuiteTypes(LoggerDestructuringConfiguration)](/LaunchPad/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions/methods.md#nettopologysuitetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`
