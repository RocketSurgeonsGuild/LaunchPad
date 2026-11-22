# NetTopologySuiteLoggerConfigurationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.spatial.md)
- Type: [NetTopologySuiteLoggerConfigurationExtensions](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.spatial/nettopologysuiteloggerconfigurationextensions.md)
- Kind: `Methods`
- Members: `1`

Adds the Destructure.NetTopologySuiteTypes() extension to `LoggerConfiguration`.

<a id="nettopologysuitetypes"></a>
<a id="nettopologysuitetypes-loggerdestructuringconfiguration"></a>

## NetTopologySuiteTypes(LoggerDestructuringConfiguration)

- Name: `NetTopologySuiteTypes(LoggerDestructuringConfiguration)`
- Modifiers: `extension`
- Returns: `LoggerConfiguration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/Spatial/NetTopologySuiteLoggerConfigurationExtensions.cs#L16)

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
