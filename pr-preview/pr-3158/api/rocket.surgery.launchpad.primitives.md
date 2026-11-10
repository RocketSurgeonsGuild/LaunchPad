# Rocket.Surgery.LaunchPad.Primitives

- Package: `Rocket.Surgery.LaunchPad.Primitives`
- Version: `2.0.1-pr.1165`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.Primitives](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Primitives)

## Namespaces

### Rocket.Surgery.LaunchPad.Primitives

- [ExistingValueOptions](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/existingvalueoptions.md) -- `class` `static` -- Helper methods for creating `Primitives.ExistingValueOptionsFactory`1` instances.
- [ExistingValueOptionsFactory<TOptions>](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/existingvalueoptionsfactory-1.md) -- `class` -- Implementation of `Options.IOptionsFactory`1`.
- [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/includeinjsoncontextattribute.md) -- `class` `sealed` -- Puts a type in the JSON context LaunchPad generates in the applications that reference this assembly, for a type that crosses the wire as JSON but isn't a mediator request or response.
- [IProblemDetailsData](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md) -- `interface` -- A common interface used for definiting problems
- [JsonContextTypeAttribute](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/jsoncontexttypeattribute.md) -- `class` `sealed` -- Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.
- [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md) -- `class` `sealed` -- Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.
- [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md) -- `record` `sealed` -- A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.
- [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/launchpadjsonenumconverters.md) -- `class` `static` -- The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.
- [LaunchPadJsonSerializerOptionsExtensions](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/launchpadjsonserializeroptionsextensions.md) -- `class` `static` -- Builds the `JsonSerializerOptions.TypeInfoResolverChain` of a `Json.JsonSerializerOptions` from registered resolvers.
- [LaunchPadJsonServiceCollectionExtensions](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/launchpadjsonservicecollectionextensions.md) -- `class` `static` -- Registers JSON type info resolvers for the `SystemTextJsonJoint` to chain into the application's `Json.JsonSerializerOptions`.
- [NotAuthorizedException](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/notauthorizedexception.md) -- `class` -- NotAuthorizedException.
- [NotFoundException](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/notfoundexception.md) -- `class` -- NotFoundException.
- [ProblemDetailsException](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/problemdetailsexception.md) -- `class` `abstract` -- NotFoundException.
- [RequestFailedException](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) -- `class` -- An exception for failed requests, with overloads to take additional details

### Rocket.Surgery.LaunchPad.Primitives.Extensions

- [UsefulEnumerableExtensions](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/usefulenumerableextensions.md) -- `class` `static` -- Useful extensions for enumerations.

### Rocket.Surgery.LaunchPad.Primitives.Joints

- [DefaultJoint](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/defaultjoint.md) -- `class` -- The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`
- [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/servicediscoverycorejoint.md) -- `class` -- A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.
- [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/standardresiliencejoint.md) -- `class` -- A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.
- [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/systemtextjsonjoint.md) -- `class` -- A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names, camel case enums, and a `JsonSerializerOptions.TypeInfoResolverChain` built from every registered [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md). Implements the `Joints.IServiceJoint`
