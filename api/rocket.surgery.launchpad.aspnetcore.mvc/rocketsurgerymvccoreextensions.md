# RocketSurgeryMvcCoreExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-beta.435`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6aecd30ca14bc14267856dbc142a55a566b9ae42/src/AspNetCore.Mvc/RocketSurgeryMvcCoreExtensions.cs)

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

- [WithMvc(IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions/methods.md#withmvc-iservicecollection) : `IMvcBuilder` `extension` -- Gets an `DependencyInjection.IMvcBuilder` with no side-effects
- [WithMvcCore(IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions/methods.md#withmvccore-iservicecollection) : `IMvcCoreBuilder` `extension` -- Gets an `DependencyInjection.IMvcCoreBuilder` with no side-effects
