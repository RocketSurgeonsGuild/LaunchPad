# JsonTypeInfoResolverRegistration Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md)
- Kind: `Properties`
- Members: `2`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="order"></a>

## Order

- Name: `Order`
- Modifiers: `get; init`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/Foundation/JsonTypeInfoResolverRegistration.cs)

Where the resolver goes in the chain: lower comes first. An application's own resolvers default to 0.

```csharp
public int Order { get; init; }
```

<a id="resolver"></a>

## Resolver

- Name: `Resolver`
- Modifiers: `get; init`
- Returns: `IJsonTypeInfoResolver`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/Foundation/JsonTypeInfoResolverRegistration.cs)

The resolver to chain.

```csharp
public IJsonTypeInfoResolver Resolver { get; init; }
```
