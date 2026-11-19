# RocketSurgeryMvcCoreExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [RocketSurgeryMvcCoreExtensions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions.md)
- Kind: `Methods`
- Members: `2`

Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.

<a id="withmvc"></a>
<a id="withmvc-iservicecollection"></a>

## WithMvc(IServiceCollection)

- Name: `WithMvc(IServiceCollection)`
- Modifiers: `extension`
- Returns: `IMvcBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.7c2af63fb1eca0ecbe24e62d859d0132ae74c3a6/src/AspNetCore.Mvc/RocketSurgeryMvcCoreExtensions.cs#L20-L21)

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

## Remarks

Adds MVC with `AddMvc` when it hasn't been added, which requires unreferenced code.

<a id="withmvccore"></a>
<a id="withmvccore-iservicecollection"></a>

## WithMvcCore(IServiceCollection)

- Name: `WithMvcCore(IServiceCollection)`
- Modifiers: `extension`
- Returns: `IMvcCoreBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.7c2af63fb1eca0ecbe24e62d859d0132ae74c3a6/src/AspNetCore.Mvc/RocketSurgeryMvcCoreExtensions.cs#L31-L32)

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
