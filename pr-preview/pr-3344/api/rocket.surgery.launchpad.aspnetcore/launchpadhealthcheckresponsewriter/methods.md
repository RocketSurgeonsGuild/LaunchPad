# LaunchPadHealthCheckResponseWriter Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckResponseWriter](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md)
- Kind: `Methods`
- Members: `2`

Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

<a id="write"></a>
<a id="write-utf8jsonwriter-healthreport-jsonserializeroptions"></a>

## Write(Utf8JsonWriter, HealthReport, JsonSerializerOptions?)

- Name: `Write(Utf8JsonWriter, HealthReport, JsonSerializerOptions?)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3344-merge.Sha.14b61fb07a6a75a780072fb07f6d7e8b006bf2cf/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs#L71-L110)

Writes the report as a JSON object.

```csharp
public static class LaunchPadHealthCheckResponseWriter
{
    public static void Write(
        Utf8JsonWriter writer,
        HealthReport report,
        JsonSerializerOptions? serializerOptions = null)
    {
        // ...
    }
}
```

## Parameters

- `writer` (`Utf8JsonWriter`)
  The writer to write to.
- `report` (`HealthReport`)
  The report to write.
- `serializerOptions` (`JsonSerializerOptions?`) `optional`
  The options whose resolvers write the health check data values, if any.

<a id="writeasync"></a>
<a id="writeasync-httpcontext-healthreport"></a>

## WriteAsync(HttpContext, HealthReport)

- Name: `WriteAsync(HttpContext, HealthReport)`
- Modifiers: `static`
- Returns: `Task`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3344-merge.Sha.14b61fb07a6a75a780072fb07f6d7e8b006bf2cf/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs#L52-L61)

Writes the report to the response. Use it as a `HealthCheckOptions.ResponseWriter`.

```csharp
public static class LaunchPadHealthCheckResponseWriter
{
    public static Task WriteAsync(
        HttpContext context,
        HealthReport report)
    {
        // ...
    }
}
```

## Parameters

- `context` (`HttpContext`)
  The context of the request.
- `report` (`HealthReport`)
  The report to write.
