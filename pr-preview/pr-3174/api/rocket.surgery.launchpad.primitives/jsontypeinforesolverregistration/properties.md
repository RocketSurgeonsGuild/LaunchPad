# JsonTypeInfoResolverRegistration Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md)
- Kind: `Properties`
- Members: `2`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="order"></a>

## Order

- Name: `Order`
- Modifiers: `get; init`
- Returns: `int`

Where the resolver goes in the chain: lower comes first. An application's own resolvers default to 0.

```csharp
public int Order { get; init; }
```

<a id="resolver"></a>

## Resolver

- Name: `Resolver`
- Modifiers: `get; init`
- Returns: `IJsonTypeInfoResolver`

The resolver to chain.

```csharp
public IJsonTypeInfoResolver Resolver { get; init; }
```
