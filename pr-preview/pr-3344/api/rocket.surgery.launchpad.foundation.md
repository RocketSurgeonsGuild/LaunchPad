# Rocket.Surgery.LaunchPad.Foundation

- Package: `Rocket.Surgery.LaunchPad.Foundation`
- Version: `2.0.1-pr.1290`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3344-merge.Sha.14b61fb07a6a75a780072fb07f6d7e8b006bf2cf/src/Foundation)
- NuGet: [Rocket.Surgery.LaunchPad.Foundation](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Foundation)

## Namespaces

### Rocket.Surgery.LaunchPad.Foundation

- [Assigned<T>](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/assigned-1.md) -- `class` -- The Assigned type is used to differentiate between not set and set input values.
- [AssignedJsonConverter<T>](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1.md) -- `class` `sealed` -- Reads and writes an `Foundation.Assigned`1` as the bare `T`: a property that is in the JSON is assigned its value, one that isn't stays unassigned (the property's own `Assigned`1.Empty`).
- [Conflict](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/conflict.md) -- `record` `sealed` -- The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.
- [ExcludeFromGenerationAttribute](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/excludefromgenerationattribute.md) -- `class` `sealed` -- Exclude the given property from the generation via launch pad source generators
- [ExistingValueOptions](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/existingvalueoptions.md) -- `class` `static` -- Helper methods for creating `Foundation.ExistingValueOptionsFactory`1` instances.
- [ExistingValueOptionsFactory<TOptions>](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/existingvalueoptionsfactory-1.md) -- `class` -- Implementation of `Options.IOptionsFactory`1`.
- [Forbidden](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/forbidden.md) -- `record` `sealed` -- The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.
- [FoundationOptions](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/foundationoptions.md) -- `class` -- Common foundation options
- [GenerationIgnoreAttribute](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/generationignoreattribute.md) -- `class` `sealed` -- Exclude the given property from the generation via launch pad source generators
- [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/includeinjsoncontextattribute.md) -- `class` `sealed` -- Puts a type in the JSON context LaunchPad generates in the applications that reference this assembly, for a type that crosses the wire as JSON but isn't a mediator request or response.
- [InheritFromAttribute](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/inheritfromattribute.md) -- `class` `sealed` -- Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
- [InheritFromAttribute<T>](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1.md) -- `class` `sealed` -- Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
- [Invalid](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/invalid.md) -- `record` `sealed` -- The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.
- [IProblemDetailsData](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata.md) -- `interface` -- A common interface used for definiting problems
- [IPropertyTracking](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/ipropertytracking.md) -- `interface` -- Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
- [IPropertyTracking<T>](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/ipropertytracking-1.md) -- `interface` -- Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
- [JsonContextTypeAttribute](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/jsoncontexttypeattribute.md) -- `class` `sealed` -- Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.
- [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md) -- `class` `sealed` -- Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.
- [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md) -- `record` `sealed` -- A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.
- [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md) -- `class` `static` -- The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.
- [LaunchPadJsonSerializerOptionsExtensions](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/launchpadjsonserializeroptionsextensions.md) -- `class` `static` -- Builds the `JsonSerializerOptions.TypeInfoResolverChain` of a `Json.JsonSerializerOptions` from registered resolvers.
- [LaunchPadJsonServiceCollectionExtensions](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/launchpadjsonservicecollectionextensions.md) -- `class` `static` -- Registers JSON type info resolvers for the `SystemTextJsonJoint` to chain into the application's `Json.JsonSerializerOptions`.
- [MutableAttribute](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/mutableattribute.md) -- `class` `sealed` -- Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of that type's properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.
- [MutableAttribute<T>](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/mutableattribute-1.md) -- `class` `sealed` -- Makes the partial class it marks a mutable view model of `T`: the generator gives the class a settable property for each of its properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. `T` can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.
- [NotFound](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/notfound.md) -- `record` `sealed` -- The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.
- [Problem](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/problem.md) -- `record` `abstract` -- The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from [IProblemDetailsData](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata.md).
- [Unauthorized](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/unauthorized.md) -- `record` `sealed` -- The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.
- [UnprocessableEntity](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/unprocessableentity.md) -- `record` `sealed` -- The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

### Rocket.Surgery.LaunchPad.Foundation.Extensions

- [UsefulEnumerableExtensions](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/usefulenumerableextensions.md) -- `class` `static` -- Useful extensions for enumerations.

### Rocket.Surgery.LaunchPad.Foundation.Joints

- [DefaultJoint](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/defaultjoint.md) -- `class` -- The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`
- [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint.md) -- `class` -- A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.
- [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/standardresiliencejoint.md) -- `class` -- A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.
- [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/systemtextjsonjoint.md) -- `class` -- A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names, camel case enums, and a `JsonSerializerOptions.TypeInfoResolverChain` built from every registered [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md). Implements the `Joints.IServiceJoint`
