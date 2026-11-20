# NodaTimeLoggerConfigurationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1254`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs)

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

- [NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) : `LoggerConfiguration` `extension` -- Enable destructuring of the NodaTime types as the text of their ISO patterns.
