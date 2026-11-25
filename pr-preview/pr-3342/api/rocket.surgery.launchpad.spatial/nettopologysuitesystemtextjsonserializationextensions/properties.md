# NetTopologySuiteSystemTextJsonSerializationExtensions Properties

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.spatial/nettopologysuitesystemtextjsonserializationextensions.md)
- Kind: `Properties`
- Members: `1`

Extensions for NetTopologySuite

<a id="typeinforesolver"></a>

## TypeInfoResolver

- Name: `TypeInfoResolver`
- Modifiers: `static` `get`
- Returns: `IJsonTypeInfoResolver`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3342-merge.Sha.fe29130e4542c32d064e5f519feab50b84c0905d/src/Spatial/NetTopologySuiteSystemTextJsonSerializationExtensions.cs#L24)

Resolves the metadata of the geometry and feature types from the converter factory in the options, without reflection, for when they are serialized at the root or as an `Object`.

```csharp
public static IJsonTypeInfoResolver TypeInfoResolver { get; }
```
