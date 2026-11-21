# LaunchPadHealthCheckOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1273`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.45fabcb1eb2968ce041767000387c82329263a76/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

Where LaunchPad maps the health check endpoints ( `MapLaunchPadHealthChecks`), and on which port.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

public sealed class LaunchPadHealthCheckOptions
{
    // ...
}
```

## Remarks

The [HealthChecksJoint](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/healthchecksjoint.md) binds these options from the [LaunchPadHealthCheckOptions.SectionName](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/fields.md#sectionname) configuration section, for example `LaunchPad__HealthChecks__Port=8081`. Code can change them with `services.Configure<LaunchPadHealthCheckOptions>`.

## Constructors

- [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/constructors.md#constructor)

## Properties

- [LivePath](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#livepath) : `string` `get; set` -- The liveness path, under [LaunchPadHealthCheckOptions.Path](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#path). The default is `live`.
- [Path](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#path) : `PathString` `get; set` -- The path of the health report, which the liveness and readiness paths sit under. The default is `/health`.
- [Port](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#port) : `int?` `get; set` -- The port the health check endpoints are served on, alone. When it is `null` (the default) they are served on the application's own addresses.
- [ReadyPath](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#readypath) : `string` `get; set` -- The readiness path, under [LaunchPadHealthCheckOptions.Path](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#path). The default is `ready`.
- [ResultStatusCodes](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#resultstatuscodes) : `IDictionary<HealthStatus, int>` `get` -- The status code each endpoint answers for each `HealthChecks.HealthStatus`. The defaults are 200 for `HealthStatus.Healthy` and `HealthStatus.Degraded`, and 503 for `HealthStatus.Unhealthy`.

## Fields

- [SectionName](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/fields.md#sectionname) : `string` `static` -- The configuration section the options are bound from.
