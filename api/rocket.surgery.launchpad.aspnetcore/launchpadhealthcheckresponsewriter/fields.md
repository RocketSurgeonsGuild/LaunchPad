# LaunchPadHealthCheckResponseWriter Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckResponseWriter](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md)
- Kind: `Fields`
- Members: `1`

Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

<a id="contenttype"></a>

## ContentType

- Name: `ContentType`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/AspNetCore/HealthChecks/LaunchPadHealthCheckResponseWriter.cs)

The content type the report is written with.

```csharp
public const static string ContentType
```
