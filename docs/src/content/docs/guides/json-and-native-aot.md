---
title: JSON and native AOT
description: Give LaunchPad the System.Text.Json metadata of your types with a source generated context, so the application trims and publishes with native AOT.
---

LaunchPad reads and writes JSON only with the JSON contexts it is given, never through reflection, in a JIT
application as much as in a trimmed or native AOT one ([ADR 0005](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0005-no-json-reflection.md)).
Every type that crosses the wire needs metadata from a source generated `JsonSerializerContext`, and a missing one fails during development
rather than after publishing.

## The generated JSON context

An executable project (a console or worker app, or an ASP.NET Core one) that references Rocket.Surgery.LaunchPad.Primitives gets a JSON
context written for it before it compiles. Every library that references Primitives lists, in its assembly, the types of it that cross the
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

`SystemTextJsonJoint` (in `Rocket.Surgery.LaunchPad.Primitives`) builds the `TypeInfoResolverChain` of every `JsonSerializerOptions` the
service provider creates from the registered resolvers. `SystemJsonTextJoint` (in `Rocket.Surgery.LaunchPad.AspNetCore`) copies those
options into the ASP.NET Core HTTP and MVC JSON options, so a context registered once reaches Minimal APIs, controllers, problem details and
the generated API endpoints.

The chain, first to last:

1. The application's resolvers (`order` 0, the default), in the order they were added. A resolver added twice is chained once.
2. LaunchPad's own resolvers (`JsonTypeInfoResolverRegistration.LibraryOrder`): the primitive and `object` values LaunchPad writes
   (problem details properties, health check data), the FluentValidation problem details, and the NodaTime and NetTopologySuite types,
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

Rocket.Surgery.LaunchPad.Primitives ships a generator that registers a camel case `JsonStringEnumConverter<TEnum>` for every enum a JSON
context of the project declares or reaches (through properties, collections and base types), and `SystemTextJsonJoint` adds those converters
to the options. Generic converters need no dynamic code, so enums are camel case strings in a native AOT application too. An enum no JSON
context reaches has no converter, and where reflection is available it is written as a number. An enum with a `[JsonConverter]` of its own
keeps it.

## The API generators

The generated Minimal API endpoints read and write through the HTTP JSON options. Generators can't see each other's output, so they can't
declare a context for you; in a project that sets `PublishAot`, `IsAotCompatible`, `PublishTrimmed` or `IsTrimmable`, **LPAD1030** names each
body and response type that no context in the project or its references declares. Set `LaunchPadApiRequireJsonContext` to `true` or `false`
to turn the check on or off for any project. See [API generators](../api-generators/#native-aot).
