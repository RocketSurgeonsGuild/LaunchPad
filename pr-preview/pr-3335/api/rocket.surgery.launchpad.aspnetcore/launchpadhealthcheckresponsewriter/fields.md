# LaunchPadHealthCheckResponseWriter Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckResponseWriter](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md)
- Kind: `Fields`
- Members: `1`

Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

<a id="contenttype"></a>

## ContentType

- Name: `ContentType`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs)

The content type the report is written with.

```csharp
public const static string ContentType
```
