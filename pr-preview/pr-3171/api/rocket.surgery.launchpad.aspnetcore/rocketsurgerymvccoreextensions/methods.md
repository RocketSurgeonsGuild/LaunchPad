# RocketSurgeryMvcCoreExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [RocketSurgeryMvcCoreExtensions](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.aspnetcore/rocketsurgerymvccoreextensions.md)
- Kind: `Methods`
- Members: `2`

Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.

<a id="withmvc"></a>
<a id="withmvc-iservicecollection"></a>

## WithMvc(IServiceCollection)

- Name: `WithMvc(IServiceCollection)`
- Modifiers: `extension`
- Returns: `IMvcBuilder`

Gets an `DependencyInjection.IMvcBuilder` with no side-effects

```csharp
public static class RocketSurgeryMvcCoreExtensions
{
    public static IMvcBuilder WithMvc(
        this IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)

<a id="withmvccore"></a>
<a id="withmvccore-iservicecollection"></a>

## WithMvcCore(IServiceCollection)

- Name: `WithMvcCore(IServiceCollection)`
- Modifiers: `extension`
- Returns: `IMvcCoreBuilder`

Gets an `DependencyInjection.IMvcCoreBuilder` with no side-effects

```csharp
public static class RocketSurgeryMvcCoreExtensions
{
    public static IMvcCoreBuilder WithMvcCore(
        this IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)
