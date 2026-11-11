# JsonTypeInfoResolverRegistration Fields

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md)
- Kind: `Fields`
- Members: `1`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="libraryorder"></a>

## LibraryOrder

- Name: `LibraryOrder`
- Modifiers: `static` `const`
- Returns: `int`

The order LaunchPad's own resolvers are registered with, behind the application's, so an application can take over any type.

```csharp
public const static int LibraryOrder
```
