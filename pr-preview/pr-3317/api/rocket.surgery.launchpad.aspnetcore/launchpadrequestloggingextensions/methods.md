# LaunchPadRequestLoggingExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadRequestLoggingExtensions](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions.md)
- Kind: `Methods`
- Members: `1`

Adds Serilog's request logging with LaunchPad's defaults.

<a id="uselaunchpadrequestlogging"></a>
<a id="uselaunchpadrequestlogging-iapplicationbuilder"></a>

## UseLaunchPadRequestLogging(IApplicationBuilder)

- Name: `UseLaunchPadRequestLogging(IApplicationBuilder)`
- Modifiers: `extension`
- Returns: `IApplicationBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.1621484e9867a9384634a5b693a135763b433b4a/src/AspNetCore/LaunchPadRequestLoggingExtensions.cs#L26-L35)

Default request logging for serilog with launchpad

```csharp
public static class LaunchPadRequestLoggingExtensions
{
    public static IApplicationBuilder UseLaunchPadRequestLogging(
        this IApplicationBuilder app)
    {
        // ...
    }
}
```

## Parameters

- `app` (`IApplicationBuilder`)

## Remarks

The request event is written to the Serilog `ILogger` in the container (Joinery.Dovetail.Serilog registers it), or to the static `Log.Logger` when there is none. Its level comes from [LaunchPadRequestLogging.DefaultGetLevel(HttpContext, double, Exception?)](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging/methods.md#defaultgetlevel-httpcontext-double-exception) and its properties from [LaunchPadRequestLogging.DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging/methods.md#defaultenrichdiagnosticcontext-idiagnosticcontext-httpcontext).
