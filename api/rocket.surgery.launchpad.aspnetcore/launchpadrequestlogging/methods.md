# LaunchPadRequestLogging Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadRequestLogging](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging.md)
- Kind: `Methods`
- Members: `2`

The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.

<a id="defaultenrichdiagnosticcontext"></a>
<a id="defaultenrichdiagnosticcontext-idiagnosticcontext-httpcontext"></a>

## DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)

- Name: `DefaultEnrichDiagnosticContext(IDiagnosticContext, HttpContext)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/AspNetCore/LaunchPadRequestLogging.cs#L22-L42)

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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/AspNetCore/LaunchPadRequestLogging.cs#L53-L61)

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
