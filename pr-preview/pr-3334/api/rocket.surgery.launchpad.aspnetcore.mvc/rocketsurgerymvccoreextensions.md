# RocketSurgeryMvcCoreExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/AspNetCore.Mvc/RocketSurgeryMvcCoreExtensions.cs)

Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc;

public static class RocketSurgeryMvcCoreExtensions
{
    // ...
}
```

## Methods

- [WithMvc(IServiceCollection)](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions/methods.md#withmvc-iservicecollection) : `IMvcBuilder` `extension` -- Gets an `DependencyInjection.IMvcBuilder` with no side-effects
- [WithMvcCore(IServiceCollection)](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions/methods.md#withmvccore-iservicecollection) : `IMvcCoreBuilder` `extension` -- Gets an `DependencyInjection.IMvcCoreBuilder` with no side-effects
