# LaunchPadHealthCheckExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1299`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/AspNetCore/LaunchPadHealthCheckExtensions.cs)

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

- [MapLaunchPadHealthChecks(IEndpointRouteBuilder)](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckextensions/methods.md#maplaunchpadhealthchecks-iendpointroutebuilder) : `IEndpointConventionBuilder` `extension` -- Maps the health report, liveness and readiness endpoints where [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md) says.
