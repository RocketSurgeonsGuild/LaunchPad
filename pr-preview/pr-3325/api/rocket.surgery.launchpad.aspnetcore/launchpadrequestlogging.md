# LaunchPadRequestLogging

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/AspNetCore/LaunchPadRequestLogging.cs)

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

- [DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging/methods.md#defaultenrichdiagnosticcontext-idiagnosticcontext-httpcontext) `static` -- Setup the `IDiagnosticContext` with default values from the request
- [DefaultGetLevel(HttpContext, double, Exception?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging/methods.md#defaultgetlevel-httpcontext-double-exception) : `LogEventLevel` `static` -- Gets the default logging level based on the response status code

## See Also

- [LaunchPadRequestLoggingExtensions.UseLaunchPadRequestLogging(IApplicationBuilder)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions/methods.md#uselaunchpadrequestlogging-iapplicationbuilder)
