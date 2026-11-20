# JsonTypeInfoResolverRegistration Fields

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md)
- Kind: `Fields`
- Members: `1`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="libraryorder"></a>

## LibraryOrder

- Name: `LibraryOrder`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/Foundation/JsonTypeInfoResolverRegistration.cs)

The order LaunchPad's own resolvers are registered with, behind the application's, so an application can take over any type.

```csharp
public const static int LibraryOrder
```
