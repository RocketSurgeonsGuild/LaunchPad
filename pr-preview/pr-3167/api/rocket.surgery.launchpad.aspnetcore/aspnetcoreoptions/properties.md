# AspNetCoreOptions Properties

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [AspNetCoreOptions](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.aspnetcore/aspnetcoreoptions.md)
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
