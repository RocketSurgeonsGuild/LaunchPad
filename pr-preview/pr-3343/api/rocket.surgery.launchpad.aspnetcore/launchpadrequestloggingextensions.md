# LaunchPadRequestLoggingExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3343-merge.Sha.686f94dbc20c569566eba31b7d55404af5ea19ed/src/AspNetCore/LaunchPadRequestLoggingExtensions.cs)

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

- [UseLaunchPadRequestLogging(IApplicationBuilder)](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions/methods.md#uselaunchpadrequestlogging-iapplicationbuilder) : `IApplicationBuilder` `extension` -- Default request logging for serilog with launchpad
