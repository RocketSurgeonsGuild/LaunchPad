# JsonTypeInfoResolverRegistration Fields

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md)
- Kind: `Fields`
- Members: `1`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="libraryorder"></a>

## LibraryOrder

- Name: `LibraryOrder`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.0d6c4ca8b5b3c5055adee085d785c5712bbc3314/src/Foundation/JsonTypeInfoResolverRegistration.cs)

The order LaunchPad's own resolvers are registered with, behind the application's, so an application can take over any type.

```csharp
public const static int LibraryOrder
```
