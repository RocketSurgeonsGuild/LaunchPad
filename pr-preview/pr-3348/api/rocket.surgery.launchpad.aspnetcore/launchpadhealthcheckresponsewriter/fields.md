# LaunchPadHealthCheckResponseWriter Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckResponseWriter](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md)
- Kind: `Fields`
- Members: `1`

Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

<a id="contenttype"></a>

## ContentType

- Name: `ContentType`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3348-merge.Sha.90dd4871ffef6c7ec9cccc3065d35dfa4da5b954/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs)

The content type the report is written with.

```csharp
public const static string ContentType
```
