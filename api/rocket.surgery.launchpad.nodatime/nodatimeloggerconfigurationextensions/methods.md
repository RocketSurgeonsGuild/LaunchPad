# NodaTimeLoggerConfigurationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeLoggerConfigurationExtensions](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions.md)
- Kind: `Methods`
- Members: `1`

Adds the Destructure.NodaTimeTypes() extension to `LoggerConfiguration`.

<a id="nodatimetypes"></a>
<a id="nodatimetypes-loggerdestructuringconfiguration"></a>

## NodaTimeTypes(LoggerDestructuringConfiguration)

- Name: `NodaTimeTypes(LoggerDestructuringConfiguration)`
- Modifiers: `extension`
- Returns: `LoggerConfiguration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.974bfd9a73f64517e8a81d86b29393f3cce75778/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs#L20-L25)

Enable destructuring of the NodaTime types as the text of their ISO patterns.

```csharp
public static class NodaTimeLoggerConfigurationExtensions
{
    public static LoggerConfiguration NodaTimeTypes(
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
