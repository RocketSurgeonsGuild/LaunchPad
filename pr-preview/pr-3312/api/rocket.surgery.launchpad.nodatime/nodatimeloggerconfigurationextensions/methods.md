# NodaTimeLoggerConfigurationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeLoggerConfigurationExtensions](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions.md)
- Kind: `Methods`
- Members: `1`

Adds the Destructure.NodaTimeTypes() extension to `LoggerConfiguration`.

<a id="nodatimetypes"></a>
<a id="nodatimetypes-loggerdestructuringconfiguration"></a>

## NodaTimeTypes(LoggerDestructuringConfiguration)

- Name: `NodaTimeTypes(LoggerDestructuringConfiguration)`
- Modifiers: `extension`
- Returns: `LoggerConfiguration`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs#L20-L25)

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
