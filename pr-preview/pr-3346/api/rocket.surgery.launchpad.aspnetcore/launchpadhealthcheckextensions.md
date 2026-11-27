# LaunchPadHealthCheckExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1292`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/AspNetCore/LaunchPadHealthCheckExtensions.cs)

Maps LaunchPad's health check endpoints.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore;

public static class LaunchPadHealthCheckExtensions
{
    // ...
}
```

## Methods

- [MapLaunchPadHealthChecks(IEndpointRouteBuilder)](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckextensions/methods.md#maplaunchpadhealthchecks-iendpointroutebuilder) : `IEndpointConventionBuilder` `extension` -- Maps the health report, liveness and readiness endpoints where [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md) says.
