# AspNetCoreMvcOptions Properties

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [AspNetCoreMvcOptions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md)
- Kind: `Properties`
- Members: `1`

Options for launchpad and aspnet core.

<a id="assemblypartfilter"></a>

## AssemblyPartFilter

- Name: `AssemblyPartFilter`
- Modifiers: `get; set`
- Returns: `Func<Assembly, bool>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/AspNetCore.Mvc/AspNetCoreMvcOptions.cs)

Defines a filter to exclude assembly parts from the application.

```csharp
public Func<Assembly, bool> AssemblyPartFilter { get; set; }
```
