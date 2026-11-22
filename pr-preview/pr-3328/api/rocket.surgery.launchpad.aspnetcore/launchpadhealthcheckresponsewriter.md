# LaunchPadHealthCheckResponseWriter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3328-merge.Sha.e3c937ccb9d716ab226fa523eee028ac17c7ed68/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs)

Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

public static class LaunchPadHealthCheckResponseWriter
{
    // ...
}
```

## Remarks

The report is written with a `Json.Utf8JsonWriter`, never through reflection, so the writer is trim and native AOT safe (ADR 0007). Each health check data value is written with the metadata the application's HTTP JSON options resolve for it ( `ConfigureHttpJsonOptions`, or a resolver added with `AddJsonTypeInfoResolver`); a value no resolver knows is written as its `Object.ToString`.

The `exception` of an entry is the whole `Exception.ToString`: its type, message, stack trace and inner exceptions. Serve the report on a port of its own ( [LaunchPadHealthCheckOptions.Port](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#port)) when it must not be public.

## Methods

- [Write(Utf8JsonWriter, HealthReport, JsonSerializerOptions?)](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter/methods.md#write-utf8jsonwriter-healthreport-jsonserializeroptions) `static` -- Writes the report as a JSON object.
- [WriteAsync(HttpContext, HealthReport)](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter/methods.md#writeasync-httpcontext-healthreport) : `Task` `static` -- Writes the report to the response. Use it as a `HealthCheckOptions.ResponseWriter`.

## Fields

- [ContentType](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter/fields.md#contenttype) : `string` `static` -- The content type the report is written with.

## Examples

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0012345",
  "entries": {
    "db": { "data": {}, "description": "fine", "duration": "00:00:00.0010000", "status": "Healthy", "tags": ["ready"] }
  }
}
```
