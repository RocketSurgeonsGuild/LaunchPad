# LaunchPadJsonServiceCollectionExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1305`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/Foundation/LaunchPadJsonServiceCollectionExtensions.cs)

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

- [AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation/launchpadjsonservicecollectionextensions/methods.md#addjsontypeinforesolver-iservicecollection-ijsontypeinforesolver-int) : `IServiceCollection` `extension` -- Adds a JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, to the front of the `JsonSerializerOptions.TypeInfoResolverChain` of the application's `Json.JsonSerializerOptions`, and of the ASP.NET Core HTTP and MVC JSON options when LaunchPad's ASP.NET Core package is used.
