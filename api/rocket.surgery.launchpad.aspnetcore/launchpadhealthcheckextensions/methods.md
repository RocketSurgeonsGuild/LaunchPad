# LaunchPadHealthCheckExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckExtensions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckextensions.md)
- Kind: `Methods`
- Members: `1`

Maps LaunchPad's health check endpoints.

<a id="maplaunchpadhealthchecks"></a>
<a id="maplaunchpadhealthchecks-iendpointroutebuilder"></a>

## MapLaunchPadHealthChecks(IEndpointRouteBuilder)

- Name: `MapLaunchPadHealthChecks(IEndpointRouteBuilder)`
- Modifiers: `extension`
- Returns: `IEndpointConventionBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.8fad8144321d59e157ec1f0adfb0c30ad2256abf/src/AspNetCore/LaunchPadHealthCheckExtensions.cs#L54-L63)

Maps the health report, liveness and readiness endpoints where [LaunchPadHealthCheckOptions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md) says.

```csharp
public static class LaunchPadHealthCheckExtensions
{
    public static IEndpointConventionBuilder MapLaunchPadHealthChecks(
        this IEndpointRouteBuilder endpoints)
    {
        // ...
    }
}
```

## Parameters

- `endpoints` (`IEndpointRouteBuilder`)
  The endpoints to map them on.

## Returns

`IEndpointConventionBuilder` -- A builder for the three endpoints.

## Remarks

| Path (by default) | What it answers |
| --- | --- |
| `/health` | Runs every health check and writes the report as JSON for the HealthChecks.UI dashboard ( [LaunchPadHealthCheckResponseWriter](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md)). |
| `/health/live` | Runs no health check: it answers `Healthy` as long as the application is up. Use it for a liveness probe. |
| `/health/ready` | Runs every health check and answers their status as plain text. Use it for a readiness probe. |

Every endpoint allows anonymous requests and answers with [LaunchPadHealthCheckOptions.ResultStatusCodes](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#resultstatuscodes). The returned builder applies its conventions to all three, so `RequireAuthorization` on it secures them all. The health check services must be registered ( `AddHealthChecks`), which LaunchPad's `HealthChecksJoint` does.
