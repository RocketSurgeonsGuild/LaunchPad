# JsonTypeInfoResolverRegistration

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Implements: `IEquatable<JsonTypeInfoResolverRegistration>`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed record JsonTypeInfoResolverRegistration
    : System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.JsonTypeInfoResolverRegistration>
{
    // ...
}
```

## Remarks

Register one with [LaunchPadJsonServiceCollectionExtensions.AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/launchpadjsonservicecollectionextensions/methods.md#addjsontypeinforesolver-iservicecollection-ijsontypeinforesolver-int). Resolvers are chained by [JsonTypeInfoResolverRegistration.Order](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/properties.md#order), lowest first, then in the order they were registered, so the first resolver that knows a type wins.

## Constructors

- [JsonTypeInfoResolverRegistration(IJsonTypeInfoResolver, int)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/constructors.md#constructor-ijsontypeinforesolver-int) -- A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

## Properties

- [Order](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/properties.md#order) : `int` `get; init` -- Where the resolver goes in the chain: lower comes first. An application's own resolvers default to 0.
- [Resolver](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/properties.md#resolver) : `IJsonTypeInfoResolver` `get; init` -- The resolver to chain.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#clone) : [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md)
- [Deconstruct(IJsonTypeInfoResolver, int)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#deconstruct-ijsontypeinforesolver-int)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(JsonTypeInfoResolverRegistration?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#equals-jsontypeinforesolverregistration) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(JsonTypeInfoResolverRegistration?, JsonTypeInfoResolverRegistration?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#op-equality-jsontypeinforesolverregistration-jsontypeinforesolverregistration) : `bool` `static`
- [op_Inequality(JsonTypeInfoResolverRegistration?, JsonTypeInfoResolverRegistration?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#op-inequality-jsontypeinforesolverregistration-jsontypeinforesolverregistration) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/methods.md#tostring) : `string` -- Returns a string that represents the current object.

## Fields

- [LibraryOrder](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration/fields.md#libraryorder) : `int` `static` -- The order LaunchPad's own resolvers are registered with, behind the application's, so an application can take over any type.
