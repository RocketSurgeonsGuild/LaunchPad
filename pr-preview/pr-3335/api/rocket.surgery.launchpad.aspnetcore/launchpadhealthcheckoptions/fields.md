# LaunchPadHealthCheckOptions Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md)
- Kind: `Fields`
- Members: `1`

Where LaunchPad maps the health check endpoints ( `MapLaunchPadHealthChecks`), and on which port.

<a id="sectionname"></a>

## SectionName

- Name: `SectionName`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The configuration section the options are bound from.

```csharp
public const static string SectionName
```
