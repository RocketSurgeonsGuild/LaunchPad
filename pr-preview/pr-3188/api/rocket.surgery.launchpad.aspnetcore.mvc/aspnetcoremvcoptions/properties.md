# AspNetCoreMvcOptions Properties

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3188/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [AspNetCoreMvcOptions](/LaunchPad/pr-preview/pr-3188/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md)
- Kind: `Properties`
- Members: `1`

Options for launchpad and aspnet core.

<a id="assemblypartfilter"></a>

## AssemblyPartFilter

- Name: `AssemblyPartFilter`
- Modifiers: `get; set`
- Returns: `Func<Assembly, bool>`

Defines a filter to exclude assembly parts from the application.

```csharp
public Func<Assembly, bool> AssemblyPartFilter { get; set; }
```
