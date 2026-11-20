# NetTopologySuiteLoggerConfigurationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteLoggerConfigurationExtensions](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions.md)
- Kind: `Methods`
- Members: `1`

Adds the Destructure.NetTopologySuiteTypes() extension to `LoggerConfiguration`.

<a id="nettopologysuitetypes"></a>
<a id="nettopologysuitetypes-loggerdestructuringconfiguration"></a>

## NetTopologySuiteTypes(LoggerDestructuringConfiguration)

- Name: `NetTopologySuiteTypes(LoggerDestructuringConfiguration)`
- Modifiers: `extension`
- Returns: `LoggerConfiguration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/Spatial/NetTopologySuiteLoggerConfigurationExtensions.cs#L16)

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
