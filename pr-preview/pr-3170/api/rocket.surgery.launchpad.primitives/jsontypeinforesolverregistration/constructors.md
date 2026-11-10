# JsonTypeInfoResolverRegistration Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.primitives.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md)
- Kind: `Constructors`
- Members: `1`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="constructor"></a>
<a id="constructor-ijsontypeinforesolver-int"></a>

## JsonTypeInfoResolverRegistration(IJsonTypeInfoResolver, int)

- Name: `Constructor(IJsonTypeInfoResolver, int)`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

```csharp
public record JsonTypeInfoResolverRegistration
{
    public JsonTypeInfoResolverRegistration(
        IJsonTypeInfoResolver Resolver,
        int Order = 0)
    {
        // ...
    }
}
```

## Parameters

- `Resolver` (`IJsonTypeInfoResolver`)
  The resolver to chain.
- `Order` (`int`) `optional`
  Where the resolver goes in the chain: lower comes first. An application's own resolvers default to 0.

## Remarks

Register one with [LaunchPadJsonServiceCollectionExtensions.AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.primitives/launchpadjsonservicecollectionextensions/methods.md#addjsontypeinforesolver-iservicecollection-ijsontypeinforesolver-int). Resolvers are chained by [JsonTypeInfoResolverRegistration.Order](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/properties.md#order), lowest first, then in the order they were registered, so the first resolver that knows a type wins.
