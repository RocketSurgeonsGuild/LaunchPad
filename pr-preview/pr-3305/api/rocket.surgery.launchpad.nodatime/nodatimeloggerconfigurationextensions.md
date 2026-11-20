# NodaTimeLoggerConfigurationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/NodaTime/NodaTimeLoggerConfigurationExtensions.cs)

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

- [NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) : `LoggerConfiguration` `extension` -- Enable destructuring of the NodaTime types as the text of their ISO patterns.
