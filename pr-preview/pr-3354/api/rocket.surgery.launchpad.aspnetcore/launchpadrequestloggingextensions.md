# LaunchPadRequestLoggingExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/AspNetCore/LaunchPadRequestLoggingExtensions.cs)

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

- [UseLaunchPadRequestLogging(IApplicationBuilder)](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions/methods.md#uselaunchpadrequestlogging-iapplicationbuilder) : `IApplicationBuilder` `extension` -- Default request logging for serilog with launchpad
