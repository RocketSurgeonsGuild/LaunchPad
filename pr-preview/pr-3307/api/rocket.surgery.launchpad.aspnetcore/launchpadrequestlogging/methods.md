# LaunchPadRequestLogging Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadRequestLogging](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging.md)
- Kind: `Methods`
- Members: `2`

The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.

<a id="defaultenrichdiagnosticcontext"></a>
<a id="defaultenrichdiagnosticcontext-idiagnosticcontext-httpcontext"></a>

## DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)

- Name: `DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/AspNetCore/LaunchPadRequestLogging.cs#L22-L42)

Setup the `IDiagnosticContext` with default values from the request

```csharp
public static class LaunchPadRequestLogging
{
    public static void DefaultEnrichDiagnosticContext(
        IDiagnosticContext diagnosticContext,
        HttpContext httpContext)
    {
        // ...
    }
}
```

## Parameters

- `diagnosticContext` (`IDiagnosticContext`)
- `httpContext` (`HttpContext`)

<a id="defaultgetlevel"></a>
<a id="defaultgetlevel-httpcontext-double-exception"></a>

## DefaultGetLevel(HttpContext, double, Exception?)

- Name: `DefaultGetLevel(HttpContext, double, Exception?)`
- Modifiers: `static`
- Returns: `LogEventLevel`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/AspNetCore/LaunchPadRequestLogging.cs#L53-L61)

Gets the default logging level based on the response status code

```csharp
public static class LaunchPadRequestLogging
{
    public static LogEventLevel DefaultGetLevel(
        HttpContext ctx,
        double _,
        Exception? ex)
    {
        // ...
    }
}
```

## Parameters

- `ctx` (`HttpContext`)
- `_` (`double`)
- `ex` (`Exception?`)
