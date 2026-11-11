# RocketSurgeryMvcCoreExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1178`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net10.0`

Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore;

public static class RocketSurgeryMvcCoreExtensions
{
    // ...
}
```

## Methods

- [WithMvc(IServiceCollection)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.aspnetcore/rocketsurgerymvccoreextensions/methods.md#withmvc-iservicecollection) : `IMvcBuilder` `extension` -- Gets an `DependencyInjection.IMvcBuilder` with no side-effects
- [WithMvcCore(IServiceCollection)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.aspnetcore/rocketsurgerymvccoreextensions/methods.md#withmvccore-iservicecollection) : `IMvcCoreBuilder` `extension` -- Gets an `DependencyInjection.IMvcCoreBuilder` with no side-effects
