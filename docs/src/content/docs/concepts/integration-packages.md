---
title: Integration packages
description: An integration package adds LaunchPad support for one third-party technology on one surface. The SDK adds them from a project's direct references.
---

An **integration package** adds LaunchPad support for one third-party technology on one surface. NodaTime has one package for
the clock and serialization, `Rocket.Surgery.LaunchPad.NodaTime`, and one for each library it works with:
`Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`, `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`,
`Rocket.Surgery.LaunchPad.NodaTime.EntityFramework` and so on. One technology usually has several.

Each package exports the [Joints](../joints/) for its pair of technologies and depends on both. Nothing in it is optional, so
a package is tested, versioned and found like any other. The alternative, one package per technology that ships its integrations
as templates, was rejected for that reason (see
[ADR 0002](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0002-package-per-integration.md)). The cost is many
small packages, which the [LaunchPad SDK](../launchpad-sdk/) chooses for you.

## The packages the SDK chooses

| The project references          | The SDK adds                                                                                 |
| ------------------------------- | -------------------------------------------------------------------------------------------- |
| a [host model](../host-models/) | `Rocket.Surgery.LaunchPad.Hosting`                                                           |
| the `WebApplication` host model | `Rocket.Surgery.LaunchPad.AspNetCore`                                                        |
| `NodaTime`                      | `.NodaTime`, and `.NodaTime.HotChocolate` / `.NodaTime.StrawberryShake` with those libraries |
| `NetTopologySuite`              | `.Spatial`, and `.StrawberryShake.Spatial` with StrawberryShake                              |
| `FluentValidation`              | `.FluentValidation`, and its ASP.NET Core, gRPC and Blazor packages with those               |
| `ZeroAlloc.Validation`          | the ZeroAlloc ASP.NET Core, gRPC, HotChocolate and Blazor packages with those                |
| `HotChocolate`                  | `.HotChocolate`                                                                              |
| `StrawberryShake`               | `.StrawberryShake`                                                                           |
| `Grpc.AspNetCore`               | `.Grpc`                                                                                      |

There is no FluentValidation package for HotChocolate: `Rocket.Surgery.LaunchPad.FluentValidation.HotChocolate` was built on
FairyBread, which has no release for HotChocolate 16, and was removed with the move to HotChocolate 16. Validate GraphQL requests
in the mediator pipeline instead (the ZeroAlloc HotChocolate package reports its failures as GraphQL errors).

### NodaTime with HotChocolate and StrawberryShake

`Rocket.Surgery.LaunchPad.NodaTime.HotChocolate` adds HotChocolate 16's own NodaTime scalars (`AddNodaTime()` from
`HotChocolate.Types.NodaTime`) and binds the same types to comparable filters:

| NodaTime type    | GraphQL scalar  | Format                                     |
| ---------------- | --------------- | ------------------------------------------ |
| `OffsetDateTime` | `DateTime`      | `2020-01-02T03:04:05.5+01:00`, `Z` for UTC |
| `Duration`       | `Duration`      | ISO 8601, `P1DT2H3M4.5S`                   |
| `LocalDate`      | `LocalDate`     | `2020-01-02`                               |
| `LocalDateTime`  | `LocalDateTime` | `2020-01-02T03:04:05.5`                    |
| `LocalTime`      | `LocalTime`     | `03:04:05.5`                               |

HotChocolate 16 renamed the `OffsetDateTime` scalar to `DateTime` and dropped the others; LaunchPad does not add them back.
`Instant`, `ZonedDateTime`, `OffsetDate`, `OffsetTime`, `Offset`, `Period`, `DateTimeZone` and `IsoDayOfWeek` have no scalar:
expose one of the types above, or bind the type to a scalar yourself, for example an `Instant` as a `DateTime` in UTC:

```csharp
builder.Services.AddGraphQLServer()
   .BindRuntimeType<Instant, HotChocolate.Types.NodaTime.DateTimeType>()
   .AddTypeConverter<Instant, OffsetDateTime>(instant => instant.WithOffset(Offset.Zero))
   .AddTypeConverter<OffsetDateTime, Instant>(dateTime => dateTime.ToInstant());
```

`Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake` registers a serializer for each of the five scalars, in the formats above. A
client maps the scalars to the NodaTime types in its `schema.extensions.graphql`, for example
`extend scalar DateTime @serializationType(name: "global::System.String") @runtimeType(name: "global::NodaTime.OffsetDateTime", valueType: true)`.

A file-based app (`dotnet run app.cs`) also gets `Rocket.Surgery.LaunchPad.Foundation` and counts as referencing NodaTime.

Each technology can be turned off with its `LaunchPadEnable<Technology>` property, which removes its own package and every
integration package it is part of:

```xml
<PropertyGroup>
  <LaunchPadEnableNodaTime>false</LaunchPadEnableNodaTime>
</PropertyGroup>
```

The properties are `LaunchPadEnableHosting`, `LaunchPadEnableFoundation`, `LaunchPadEnableAspNetCore`, `LaunchPadEnableNodaTime`,
`LaunchPadEnableNetTopologySuite`, `LaunchPadEnableFluentValidation`, `LaunchPadEnableZeroAlloc`, `LaunchPadEnableGrpc`, `LaunchPadEnableHotChocolate` and
`LaunchPadEnableStrawberryShake`.

## Direct references

A **direct reference** is a package or project reference a project declares itself, as opposed to one it only receives through
another package or project. The SDK and the [templates](../templates/) act on direct references only: a library the project
receives transitively was chosen by whoever referenced it, not by the project, so it adds nothing to the project.

If a technology arrives only transitively and you want its LaunchPad support, reference it directly.
