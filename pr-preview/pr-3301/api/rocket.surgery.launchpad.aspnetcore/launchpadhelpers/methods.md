# LaunchPadHelpers Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHelpers](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.aspnetcore/launchpadhelpers.md)
- Kind: `Methods`
- Members: `1`

Helpers for during application startup and in middleware

<a id="defaultresponsewriter"></a>
<a id="defaultresponsewriter-httpcontext-healthreport"></a>

## DefaultResponseWriter(HttpContext, HealthReport)

- Name: `DefaultResponseWriter(HttpContext, HealthReport)`
- Modifiers: `static`
- Returns: `Task`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/AspNetCore/LaunchPadHelpers.cs#L30-L102)

The default response writer for health checks

```csharp
public static class LaunchPadHelpers
{
    public static Task DefaultResponseWriter(
        HttpContext context,
        HealthReport report)
    {
        // ...
    }
}
```

## Parameters

- `context` (`HttpContext`)
- `report` (`HealthReport`)

## Remarks

Each health check data value is written with the metadata the application's HTTP JSON options resolve for it ( `ConfigureHttpJsonOptions`, or a resolver added with `AddJsonTypeInfoResolver`), so the writer stays trim and native AOT safe. A value no resolver knows is written as its `Object.ToString`.
