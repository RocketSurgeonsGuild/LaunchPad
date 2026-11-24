# JSON and native AOT

LaunchPad reads and writes JSON only with the JSON contexts it is given, never through reflection, in a JIT
application as much as in a trimmed or native AOT one ([ADR 0007](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0007-no-json-reflection.md)).
Every type that crosses the wire needs metadata from a source generated `JsonSerializerContext`, and a missing one fails during development
rather than after publishing.

## The generated JSON context

An executable project (a console or worker app, or an ASP.NET Core one) that references Rocket.Surgery.LaunchPad.Foundation gets a JSON
context written for it before it compiles. Every library that references Foundation lists, in its assembly, the types of it that cross the
wire as JSON:

- every mediator request, with its response or stream item type,
- every `IPropertyTracking<T>` patch type, with its target,
- every type marked `[IncludeInJsonContext]`.

The application's build reads those lists from its referenced assemblies and writes `LaunchPadJsonContext` (in the project's root
namespace), with a `[JsonSerializable]` for each type, and a Joint that chains it into the application's JSON options. Nothing needs
wiring.

The context is written as source, not by a generator, because generators can't see each other's output: the System.Text.Json source
generator would never see a context another generator wrote. And it names types from referenced assemblies, because their metadata is
complete. A type compiled in the same project as the context only has the members its source declares, not the ones other generators
add to it (property tracking, `[InheritFrom]`). So keep operations in a contracts or core project. Requests declared in the application
itself are still reported by LPAD1030.

| Property                        | Default                                     | Meaning            |
| ------------------------------- | ------------------------------------------- | ------------------ |
| `LaunchPadGenerateJsonContext`  | `true` in executables, not in test projects | Write the context. |
| `LaunchPadJsonContextName`      | `LaunchPadJsonContext`                      | The class name.    |
| `LaunchPadJsonContextNamespace` | `$(RootNamespace)`                          | The namespace.     |

## Chaining a context of your own

For types the generated context doesn't cover, declare a context and register it, in a [Joint](../../concepts/joints/) or in the host's services:

```csharp
[JsonSerializable(typeof(RocketModel))]
[JsonSerializable(typeof(CreateRocket.Request), TypeInfoPropertyName = "CreateRocketRequest")]
[JsonSerializable(typeof(CreateRocket.Response), TypeInfoPropertyName = "CreateRocketResponse")]
public partial class RocketsJsonContext : JsonSerializerContext;

services.AddJsonTypeInfoResolver(RocketsJsonContext.Default);
```

`SystemTextJsonJoint` (in `Rocket.Surgery.LaunchPad.Foundation`) builds the `TypeInfoResolverChain` of every `JsonSerializerOptions` the
service provider creates from the registered resolvers. `SystemJsonTextJoint` (in `Rocket.Surgery.LaunchPad.AspNetCore`) copies those
options into the ASP.NET Core HTTP JSON options, and `MvcSystemJsonTextJoint` (in `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`) into MVC's, so a
context registered once reaches Minimal APIs, controllers, problem details and the generated API endpoints.

The chain, first to last:

1. The application's resolvers (`order` 0, the default), in the order they were added. A resolver added twice is chained once.
2. LaunchPad's own resolvers (`JsonTypeInfoResolverRegistration.LibraryOrder`): the primitive and `object` values LaunchPad writes
   (problem details properties, health check data) and the NodaTime and NetTopologySuite types,
   which resolve from the converters their packages add.

The first resolver that knows a type wins, so an application can take over any type LaunchPad resolves. LaunchPad adds no reflection based
resolver: a type no JSON context knows throws `NotSupportedException`. An application that wants reflection adds
`DefaultJsonTypeInfoResolver` to the chain itself, and LaunchPad leaves it there.

ASP.NET Core puts a reflection based resolver in the HTTP JSON options of a JIT application. `SystemJsonTextJoint` takes it out, so Minimal
APIs, problem details, OpenAPI and the generated endpoints fail on a type no JSON context knows during development, the way a native AOT
publish would. To keep it, set the MSBuild property `LaunchPadAspNetCoreJsonReflection` to `true` (or the `AppContext` switch
`SystemJsonTextJoint.KeepReflectionSwitch` before the host is built). MVC's JSON options keep the reflection resolver: MVC doesn't support
native AOT, and its model binding and formatters read types no context declares.

## Types written by a converter

A converter in `JsonSerializerOptions.Converters` handles a type, but a resolver still has to hand out its metadata when the type is
serialized at the root or as an `object`. `JsonConverterTypeInfoResolver` builds it from the converter, without reflection:

```csharp
services.AddJsonTypeInfoResolver(new JsonConverterTypeInfoResolver().AddNullable<RocketId>().Add<Trajectory>());
```

## Enums

Rocket.Surgery.LaunchPad.Foundation ships a generator that registers a camel case `JsonStringEnumConverter<TEnum>` for every enum a JSON
context of the project declares or reaches (through properties, collections and base types), and `SystemTextJsonJoint` adds those converters
to the options. Generic converters need no dynamic code, so enums are camel case strings in a native AOT application too. An enum no JSON
context reaches has no converter, and where reflection is available it is written as a number. An enum with a `[JsonConverter]` of its own
keeps it.

## The API generators

The generated Minimal API endpoints read and write through the HTTP JSON options. Generators can't see each other's output, so they can't
declare a context for you; **LPAD1030** names each body and response type that no context in the project or its references declares, in
every project, and its code fix adds them to the project's context (or creates one). Set `LaunchPadApiRequireJsonContext` to `false` to turn
the check off. See [API generators](../api-generators/#native-aot).

## Package compatibility

What works under native AOT lives in its own packages. A package that sets `IsAotCompatible` has no trimming or native AOT warning in its
own code; where a dependency can't be AOT compatible, the part that needs it is in a separate package named for what it is, which references
the AOT compatible one. A package whose dependencies aren't usable under native AOT doesn't claim it.

| Package                                    | Native AOT | Notes                                                                                                                                                                                        |
| ------------------------------------------ | ---------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Rocket.Surgery.LaunchPad.Foundation`      | yes        |                                                                                                                                                                                              |
| `Rocket.Surgery.LaunchPad.Primitives`      | yes        |                                                                                                                                                                                              |
| `Rocket.Surgery.LaunchPad.AspNetCore`      | yes        | Minimal APIs, problem details, OpenAPI, the HTTP JSON options and Serilog request logging.                                                                                                   |
| `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`  | **no**     | MVC Core with application parts, the MVC API explorer, MVC's JSON options and client error mapping, Serilog filters for actions and pages. ASP.NET Core doesn't support MVC with native AOT. |
| `Rocket.Surgery.LaunchPad.Grpc`            | yes        | ASP.NET Core gRPC supports native AOT. gRPC services generated code first from operation hosts use protobuf-net's runtime model, which doesn't.                                              |
| `Rocket.Surgery.LaunchPad.StrawberryShake` | yes        |                                                                                                                                                                                              |
| `Rocket.Surgery.LaunchPad.NodaTime`        | yes        | LaunchPad's own System.Text.Json converters for every NodaTime type; NodaTime.Serialization.SystemTextJson isn't AOT compatible yet.                                                         |
| `Rocket.Surgery.LaunchPad.Spatial`         | yes        | Geometries only: GeoJSON features aren't supported under native AOT (below).                                                                                                                 |
| `Rocket.Surgery.LaunchPad.HotChocolate`    | **no**     | Hot Chocolate's server isn't usable under native AOT yet (below).                                                                                                                            |

`test/Api/Aot/Packages.Aot` roots the AOT compatible packages and publishes them with `PublishAot` on every pull request that changes them.
It fails on any warning except the dependency warnings `test/Api/Aot/Packages.Aot/dependency-warnings.txt` lists, which are the ones below.

### What isn't supported under native AOT

**MVC.** ASP.NET Core doesn't support controllers, Razor Pages or views with native AOT. Reference `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`
only in an application that uses them; `Rocket.Surgery.LaunchPad.AspNetCore` alone gives Minimal APIs, problem details and OpenAPI.

**Hot Chocolate.** Most Hot Chocolate packages declare `IsAotCompatible`, but `HotChocolate.Types` still produces trimming and AOT warnings,
`HotChocolate.Data` (filtering, sorting, projections) and `GreenDonut` declare nothing, and a native AOT server fails at startup unless
`HotChocolate.Types` is kept from trimming
([ChilliCream/graphql-platform#10566](https://github.com/ChilliCream/graphql-platform/issues/10566)). `Rocket.Surgery.LaunchPad.HotChocolate`
uses `HotChocolate.Data` to name nested filter and sort types, so it doesn't claim `IsAotCompatible`. Strong IDs need nothing from it:
the [Primitives HotChocolate Joint](../../concepts/domain-primitives/#in-openapi-graphql-ef-core-and-dapper) makes each one an `ID`
scalar (by default) from a registry written at build time.

**GeoJSON features.** `Rocket.Surgery.LaunchPad.Spatial` adds NetTopologySuite.IO.GeoJSON4STJ's converters. Its geometry converters are AOT
safe; its converters for `IFeature`, `FeatureCollection` and `IAttributesTable` serialize the id, the attributes and the geometry with
`JsonSerializer` by their runtime type, so a native AOT publish warns (IL2104, IL3053 for `NetTopologySuite.IO.GeoJSON4STJ`). Under native AOT
they only work for values a JSON context in the options knows, and anything else throws `NotSupportedException`. Serialize geometries, or a
type of your own that holds one, rather than features. No System.Text.Json GeoJSON library is AOT compatible today.

**Code first gRPC services.** The gRPC services LaunchPad generates from operation hosts serialize with protobuf-net's runtime model, which
reflects over contracts and emits IL, so they don't work under native AOT. `Rocket.Surgery.LaunchPad.Grpc` itself is AOT compatible.

The NodaTime converters are LaunchPad's own until NodaTime.Serialization.SystemTextJson supports native AOT
([nodatime/nodatime.serialization#121](https://github.com/nodatime/nodatime.serialization/issues/121)): its `Interval` and `DateInterval`
converters look up the `Instant` and `LocalDate` converters with `JsonSerializerOptions.GetConverter(Type)`, which requires unreferenced and
dynamic code. LaunchPad's write the same JSON.