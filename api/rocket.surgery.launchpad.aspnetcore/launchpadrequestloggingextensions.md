# LaunchPadRequestLoggingExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-beta.437`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.42409360cf41d0e5a89b31252cf8cea56b207c47/src/AspNetCore/LaunchPadRequestLoggingExtensions.cs)

Adds Serilog's request logging with LaunchPad's defaults.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore;

public static class LaunchPadRequestLoggingExtensions
{
    // ...
}
```

## Methods

- [UseLaunchPadRequestLogging(IApplicationBuilder)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions/methods.md#uselaunchpadrequestlogging-iapplicationbuilder) : `IApplicationBuilder` `extension` -- Default request logging for serilog with launchpad
