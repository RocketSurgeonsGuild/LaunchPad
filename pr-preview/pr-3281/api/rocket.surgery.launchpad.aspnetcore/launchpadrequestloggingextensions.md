# LaunchPadRequestLoggingExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/AspNetCore/LaunchPadRequestLoggingExtensions.cs)

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

- [UseLaunchPadRequestLogging(IApplicationBuilder)](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions/methods.md#uselaunchpadrequestlogging-iapplicationbuilder) : `IApplicationBuilder` `extension` -- Default request logging for serilog with launchpad
