# NodaTimeLoggerConfigurationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1272`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3316-merge.Sha.4725c0a09120ecd23f44a0d7d9544708e7a3dca8/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs)

Adds the Destructure.NodaTimeTypes() extension to `LoggerConfiguration`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime;

public static class NodaTimeLoggerConfigurationExtensions
{
    // ...
}
```

## Methods

- [NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) : `LoggerConfiguration` `extension` -- Enable destructuring of the NodaTime types as the text of their ISO patterns.
