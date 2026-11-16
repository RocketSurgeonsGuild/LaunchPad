---
title: JSON and native AOT
description: Give LaunchPad the System.Text.Json metadata of your types with a source generated context, so the application trims and publishes with native AOT.
---

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
options into the ASP.NET Core HTTP and MVC JSON options, so a context registered once reaches Minimal APIs, controllers, problem details and
the generated API endpoints.

The chain, first to last:

1. The application's resolvers (`order` 0, the default), in the order they were added. A resolver added twice is chained once.
2. LaunchPad's own resolvers (`JsonTypeInfoResolverRegistration.LibraryOrder`): the primitive and `object` values LaunchPad writes
   (problem details properties, health check data) and the NodaTime and NetTopologySuite types,
   which resolve from the converters their packages add.

The first resolver that knows a type wins, so an application can take over any type LaunchPad resolves. LaunchPad adds no reflection based
resolver: a type no JSON context knows throws `NotSupportedException`. An application that wants reflection adds
`DefaultJsonTypeInfoResolver` to the chain itself, and LaunchPad leaves it there. ASP.NET Core's own HTTP and MVC JSON options keep the
reflection resolver ASP.NET adds to a JIT application; LaunchPad's resolvers go in front of it.

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
declare a context for you; in a project that sets `PublishAot`, `IsAotCompatible`, `PublishTrimmed` or `IsTrimmable`, **LPAD1030** names each
body and response type that no context in the project or its references declares. Set `LaunchPadApiRequireJsonContext` to `true` or `false`
to turn the check on or off for any project. See [API generators](../api-generators/#native-aot).

## Package compatibility

These packages set `IsAotCompatible`: their own code has no trimming or native AOT warning, and where reflection can't be avoided the API
says so with `[RequiresUnreferencedCode]`.

| Package                                    | Notes                                                                                                    |
| ------------------------------------------ | -------------------------------------------------------------------------------------------------------- |
| `Rocket.Surgery.LaunchPad.Foundation`      |                                                                                                          |
| `Rocket.Surgery.LaunchPad.Primitives`      |                                                                                                          |
| `Rocket.Surgery.LaunchPad.NodaTime`        |                                                                                                          |
| `Rocket.Surgery.LaunchPad.Spatial`         |                                                                                                          |
| `Rocket.Surgery.LaunchPad.AspNetCore`      | `WithMvc` requires unreferenced code: it adds MVC with `AddMvc` when nothing has. `WithMvcCore` doesn't. |
| `Rocket.Surgery.LaunchPad.Grpc`            |                                                                                                          |
| `Rocket.Surgery.LaunchPad.HotChocolate`    | `ConfigureStronglyTypedId` takes the conversions to and from the id's value instead of finding them.     |
| `Rocket.Surgery.LaunchPad.StrawberryShake` |                                                                                                          |

`test/Api/Aot/Packages.Aot` roots these assemblies and publishes with `PublishAot` on every pull request that changes them. It fails on a
warning from LaunchPad's code, and on a dependency warning that `test/Api/Aot/Packages.Aot/dependency-warnings.txt` doesn't list.

`AspNetCoreJoint` adds MVC's API explorer, which requires unreferenced code because it describes controllers, found by reflection. It
describes nothing in an application without controllers, and a native AOT application can't have them, so the joint suppresses that one
warning; Minimal APIs are described by the endpoint API explorer, which is trim safe.

### Dependency warnings

An application that references these packages and publishes with native AOT gets the warnings of the dependencies below. Each comes from a
library LaunchPad depends on, not from LaunchPad's code, so LaunchPad can't fix it; the publish reports them, by default once per assembly
(IL2104 for trimming, IL3053 for native AOT; set `TrimmerSingleWarn` to `false` to see each one).

| Package                                 | Dependency                                                                                                               | What warns                                                                                                                                                                                                                                                                                   | What to expect                                                                                                                                                                                                                                                                                                                   |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Rocket.Surgery.LaunchPad.AspNetCore`   | MVC (`Microsoft.AspNetCore.Mvc.*`, and `Microsoft.AspNetCore.Components.Endpoints`, which MVC's view features reference) | `AspNetCoreJoint` registers MVC Core and the API explorer, and `SerilogRequestLoggingJoint` adds MVC action and page filters. MVC's controller discovery, model binding, formatters and views use reflection (IL2026, IL3050), and related assemblies read `Assembly.Location` (IL3000).     | Minimal APIs, problem details, OpenAPI and the generated API endpoints work. Controllers, Razor Pages and views don't: ASP.NET Core doesn't support MVC with native AOT. The warnings are reported even when the application has no controllers.                                                                                 |
| `Rocket.Surgery.LaunchPad.HotChocolate` | Hot Chocolate (`HotChocolate.Types`, `HotChocolate.Data`, `GreenDonut`)                                                  | Hot Chocolate builds its schema by reflecting over the runtime types (`DefaultTypeInspector`), makes generic types and methods at runtime (`MakeGenericType`, `MakeGenericMethod`), creates objects with `Activator`, and reads XML documentation from `Assembly.Location` (IL3000, IL3002). | Hot Chocolate's schema building isn't annotated for trimming. A schema may lose members the trimmer removed, or fail at startup when a generic instantiation has no native code. Test the whole schema in the published application before relying on it, or don't trim a GraphQL server.                                        |
| `Rocket.Surgery.LaunchPad.Spatial`      | `NetTopologySuite.IO.GeoJSON4STJ`                                                                                        | The GeoJSON converters for features, feature collections and attribute tables serialize their members with `JsonSerializer` and the options they are given (IL2026, IL3050).                                                                                                                 | Geometries are written by the converters themselves. Features, feature collections and attribute tables go back to `JsonSerializer` for their attributes and geometry, which works when a JSON context in the options knows the attribute values' types; an attribute of a type no context knows throws `NotSupportedException`. |
| `Rocket.Surgery.LaunchPad.NodaTime`     | `NodaTime.Serialization.SystemTextJson`                                                                                  | The converters for composite values (`Interval`, `DateInterval`) read and write their parts with `JsonSerializerOptions.GetConverter(Type)` (IL2026, IL3050).                                                                                                                                | `LaunchPad.NodaTime` adds a converter for every NodaTime type to the options, so `GetConverter` finds the part's converter there rather than building one by reflection. The analyzer can't see that, so the warning stays.                                                                                                      |

`Rocket.Surgery.LaunchPad.Grpc` and `Rocket.Surgery.LaunchPad.StrawberryShake` add no dependency warning of their own beyond ASP.NET Core's.
