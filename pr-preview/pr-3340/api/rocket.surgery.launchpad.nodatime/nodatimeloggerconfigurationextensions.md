# NodaTimeLoggerConfigurationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3340-merge.Sha.1fd0124e0c52f2b8f0975bb6854b707a6a798292/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs)

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

- [NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) : `LoggerConfiguration` `extension` -- Enable destructuring of the NodaTime types as the text of their ISO patterns.
