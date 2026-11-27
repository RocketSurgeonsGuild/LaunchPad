# LaunchPadHealthCheckOptions Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckOptions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md)
- Kind: `Fields`
- Members: `1`

Where LaunchPad maps the health check endpoints ( `MapLaunchPadHealthChecks`), and on which port.

<a id="sectionname"></a>

## SectionName

- Name: `SectionName`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.cd7db14e49b12478b13b2f21edf214c613078b4c/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The configuration section the options are bound from.

```csharp
public const static string SectionName
```
