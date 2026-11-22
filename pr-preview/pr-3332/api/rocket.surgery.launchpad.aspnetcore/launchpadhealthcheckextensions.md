# LaunchPadHealthCheckExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/AspNetCore/LaunchPadHealthCheckExtensions.cs)

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

- [MapLaunchPadHealthChecks(IEndpointRouteBuilder)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckextensions/methods.md#maplaunchpadhealthchecks-iendpointroutebuilder) : `IEndpointConventionBuilder` `extension` -- Maps the health report, liveness and readiness endpoints where [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md) says.
