# RocketSurgeryMvcCoreExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-pr.1305`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/AspNetCore.Mvc/RocketSurgeryMvcCoreExtensions.cs)

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

- [WithMvc(IServiceCollection)](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions/methods.md#withmvc-iservicecollection) : `IMvcBuilder` `extension` -- Gets an `DependencyInjection.IMvcBuilder` with no side-effects
- [WithMvcCore(IServiceCollection)](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions/methods.md#withmvccore-iservicecollection) : `IMvcCoreBuilder` `extension` -- Gets an `DependencyInjection.IMvcCoreBuilder` with no side-effects
