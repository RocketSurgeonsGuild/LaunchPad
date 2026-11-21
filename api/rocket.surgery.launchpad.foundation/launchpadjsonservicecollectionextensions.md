# LaunchPadJsonServiceCollectionExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.441`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Foundation/LaunchPadJsonServiceCollectionExtensions.cs)

Registers JSON type info resolvers for the `SystemTextJsonJoint` to chain into the application's `Json.JsonSerializerOptions`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public static class LaunchPadJsonServiceCollectionExtensions
{
    // ...
}
```

## Methods

- [AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)](/LaunchPad/api/rocket.surgery.launchpad.foundation/launchpadjsonservicecollectionextensions/methods.md#addjsontypeinforesolver-iservicecollection-ijsontypeinforesolver-int) : `IServiceCollection` `extension` -- Adds a JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, to the front of the `JsonSerializerOptions.TypeInfoResolverChain` of the application's `Json.JsonSerializerOptions`, and of the ASP.NET Core HTTP and MVC JSON options when LaunchPad's ASP.NET Core package is used.
