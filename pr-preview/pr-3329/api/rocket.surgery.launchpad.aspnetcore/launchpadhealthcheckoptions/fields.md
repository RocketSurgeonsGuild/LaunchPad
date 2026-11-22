# LaunchPadHealthCheckOptions Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md)
- Kind: `Fields`
- Members: `1`

Where LaunchPad maps the health check endpoints ( `MapLaunchPadHealthChecks`), and on which port.

<a id="sectionname"></a>

## SectionName

- Name: `SectionName`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The configuration section the options are bound from.

```csharp
public const static string SectionName
```
