# AspNetCoreMvcOptions Properties

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [AspNetCoreMvcOptions](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md)
- Kind: `Properties`
- Members: `1`

Options for launchpad and aspnet core.

<a id="assemblypartfilter"></a>

## AssemblyPartFilter

- Name: `AssemblyPartFilter`
- Modifiers: `get; set`
- Returns: `Func<Assembly, bool>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/AspNetCore.Mvc/AspNetCoreMvcOptions.cs)

Defines a filter to exclude assembly parts from the application.

```csharp
public Func<Assembly, bool> AssemblyPartFilter { get; set; }
```
