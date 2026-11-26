# LaunchPadHealthCheckResponseWriter Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckResponseWriter](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md)
- Kind: `Fields`
- Members: `1`

Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

<a id="contenttype"></a>

## ContentType

- Name: `ContentType`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs)

The content type the report is written with.

```csharp
public const static string ContentType
```
