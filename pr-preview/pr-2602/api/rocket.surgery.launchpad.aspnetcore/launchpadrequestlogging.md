# LaunchPadRequestLogging

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/AspNetCore/LaunchPadRequestLogging.cs)

The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore;

public static class LaunchPadRequestLogging
{
    // ...
}
```

## Methods

- [DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging/methods.md#defaultenrichdiagnosticcontext-idiagnosticcontext-httpcontext) `static` -- Setup the `IDiagnosticContext` with default values from the request
- [DefaultGetLevel(HttpContext, double, Exception?)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging/methods.md#defaultgetlevel-httpcontext-double-exception) : `LogEventLevel` `static` -- Gets the default logging level based on the response status code

## See Also

- [LaunchPadRequestLoggingExtensions.UseLaunchPadRequestLogging(IApplicationBuilder)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions/methods.md#uselaunchpadrequestlogging-iapplicationbuilder)
