# NetTopologySuiteLoggerConfigurationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteLoggerConfigurationExtensions](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions.md)
- Kind: `Methods`
- Members: `1`

Adds the Destructure.NetTopologySuiteTypes() extension to `LoggerConfiguration`.

<a id="nettopologysuitetypes"></a>
<a id="nettopologysuitetypes-loggerdestructuringconfiguration"></a>

## NetTopologySuiteTypes(LoggerDestructuringConfiguration)

- Name: `NetTopologySuiteTypes(LoggerDestructuringConfiguration)`
- Modifiers: `extension`
- Returns: `LoggerConfiguration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/Spatial/NetTopologySuiteLoggerConfigurationExtensions.cs#L16)

Enable destructuring of the NetTopologySuite geometries, features and attribute tables.

```csharp
public static class NetTopologySuiteLoggerConfigurationExtensions
{
    public static LoggerConfiguration NetTopologySuiteTypes(
        this LoggerDestructuringConfiguration configuration)
    {
        // ...
    }
}
```

## Parameters

- `configuration` (`LoggerDestructuringConfiguration`)
  The logger configuration to apply configuration to.

## Returns

`LoggerConfiguration` -- An object allowing configuration to continue.
