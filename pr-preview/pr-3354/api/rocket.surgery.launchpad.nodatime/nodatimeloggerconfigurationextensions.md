# NodaTimeLoggerConfigurationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs)

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

- [NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) : `LoggerConfiguration` `extension` -- Enable destructuring of the NodaTime types as the text of their ISO patterns.
