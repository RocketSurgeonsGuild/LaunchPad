# NetTopologySuiteSystemTextJsonSerializationExtensions Properties

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions.md)
- Kind: `Properties`
- Members: `1`

Extensions for NetTopologySuite

<a id="typeinforesolver"></a>

## TypeInfoResolver

- Name: `TypeInfoResolver`
- Modifiers: `static` `get`
- Returns: `IJsonTypeInfoResolver`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs#L24)

Resolves the metadata of the geometry and feature types from the converter factory in the options, without reflection, for when they are serialized at the root or as an `Object`.

```csharp
public static IJsonTypeInfoResolver TypeInfoResolver { get; }
```
