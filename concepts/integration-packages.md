# Integration packages

An **integration package** adds LaunchPad support for one third-party technology on one surface. NodaTime has one package for
the clock and serialization, `Rocket.Surgery.LaunchPad.NodaTime`, and one for each library it works with:
`Rocket.Surgery.LaunchPad.NodaTime.HotChocolate` and `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`. One technology usually
has several.

Each package exports the [Joints](../joints/) for its pair of technologies and depends on both. Nothing in it is optional, so
a package is tested, versioned and found like any other. The alternative, one package per technology that ships its integrations
as templates, was rejected for that reason (see
[ADR 0002](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0002-package-per-integration.md)). The cost is many
small packages, which the [LaunchPad SDK](../launchpad-sdk/) chooses for you.

## The packages the SDK chooses

| The project references          | The SDK adds                                                                                          |
| ------------------------------- | ----------------------------------------------------------------------------------------------------- |
| a [host model](../host-models/) | `Rocket.Surgery.LaunchPad.Hosting`                                                                    |
| the `WebApplication` host model | `Rocket.Surgery.LaunchPad.AspNetCore`, and `.AspNetCore.Mvc` unless `PublishAot` or `IsAotCompatible` |
| `NodaTime`                      | `.NodaTime`, and `.NodaTime.HotChocolate` / `.NodaTime.StrawberryShake` with those libraries          |
| `NetTopologySuite`              | `.Spatial`, and `.StrawberryShake.Spatial` with StrawberryShake                                       |
| `ZeroAlloc.Validation`          | the ZeroAlloc ASP.NET Core, gRPC, HotChocolate and Blazor packages with those                         |
| `HotChocolate`                  | `.HotChocolate`                                                                                       |
| `StrawberryShake`               | `.StrawberryShake`                                                                                    |
| `Grpc.AspNetCore`               | `.Grpc`                                                                                               |

Validation is ZeroAlloc.Validation's. With ZeroAlloc.Mediator, its validation behavior checks each request against the request's
rules and throws a `ValidationFailedException`, and the ZeroAlloc package of each transport reports the failures in that
transport's error shape (problem details over HTTP, a status over gRPC, GraphQL errors over HotChocolate). The other mediators
have no validation step that LaunchPad hooks into: validate in the handler and return an error case such as `Invalid` or
`UnprocessableEntity` (see [union responses](../../guides/api-generators/#union-responses)).

### NodaTime with HotChocolate and StrawberryShake

`Rocket.Surgery.LaunchPad.NodaTime.HotChocolate` adds HotChocolate 16's own NodaTime scalars (`AddNodaTime()` from
`HotChocolate.Types.NodaTime`), binds `Instant` to the `DateTime` scalar and binds all of these types to comparable filters:

| NodaTime type    | GraphQL scalar  | Format                                                         |
| ---------------- | --------------- | -------------------------------------------------------------- |
| `OffsetDateTime` | `DateTime`      | `2020-01-02T03:04:05.5+01:00`, `Z` for UTC                     |
| `Instant`        | `DateTime`      | written in UTC, `2020-01-02T02:04:05.5Z`; read with any offset |
| `Duration`       | `Duration`      | ISO 8601, `P1DT2H3M4.5S`                                       |
| `LocalDate`      | `LocalDate`     | `2020-01-02`                                                   |
| `LocalDateTime`  | `LocalDateTime` | `2020-01-02T03:04:05.5`                                        |
| `LocalTime`      | `LocalTime`     | `03:04:05.5`                                                   |

An `Instant` is an `OffsetDateTime` at UTC, so it travels as the `DateTime` scalar without losing anything: the package converts
it with `instant.WithOffset(Offset.Zero)` on the way out and `dateTime.ToInstant()` on the way in, so a `DateTime` sent with an
offset arrives as the same instant.

HotChocolate 16 renamed the `OffsetDateTime` scalar to `DateTime` and dropped the others; LaunchPad does not add them back.
`ZonedDateTime`, `OffsetDate`, `OffsetTime`, `Offset`, `Period`, `DateTimeZone` and `IsoDayOfWeek` have no scalar: expose one of
the types above, or bind the type to a scalar yourself with `BindRuntimeType` and a pair of `AddTypeConverter` calls.

`Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake` registers a serializer for each of the five scalars, in the formats above. A
client maps the scalars to the NodaTime types in its `schema.extensions.graphql`, for example
`extend scalar DateTime @serializationType(name: "global::System.String") @runtimeType(name: "global::NodaTime.OffsetDateTime", valueType: true)`.

A file-based app (`dotnet run app.cs`) also gets `Rocket.Surgery.LaunchPad.Foundation` and counts as referencing NodaTime.

### OpenTelemetry

OpenTelemetry follows the host model rather than a reference: every application that gets `Rocket.Surgery.LaunchPad.Hosting` gets
its traces, metrics and logs, with the packages that turn on LaunchPad.Hosting's OpenTelemetry joints
(see [templates](../templates/#the-hosting-templates)):

| Package                                        | Host models                                            |
| ---------------------------------------------- | ------------------------------------------------------ |
| `Joinery.Dovetail.OpenTelemetry`               | generic host, ASP.NET Core, Blazor WebAssembly         |
| `OpenTelemetry.Instrumentation.Http`           | generic host, ASP.NET Core, Blazor WebAssembly         |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | generic host, ASP.NET Core, Blazor WebAssembly         |
| `OpenTelemetry.Extensions.Hosting`             | generic host, ASP.NET Core                             |
| `OpenTelemetry.Instrumentation.Runtime`        | generic host, ASP.NET Core                             |
| `OpenTelemetry.Instrumentation.Process`        | generic host, ASP.NET Core                             |
| `OpenTelemetry.Instrumentation.AspNetCore`     | ASP.NET Core                                           |
| `Serilog.Sinks.OpenTelemetry`                  | generic host, ASP.NET Core, when it references Serilog |

The exporters only export once `OTEL_EXPORTER_OTLP_ENDPOINT` is set, as Aspire sets it, so an application run on its own sends
nothing. `LaunchPadEnableOtlpExporter` set to `false` leaves the exporter and Serilog's sink out and keeps the instrumentation;
`LaunchPadEnableOpenTelemetry` set to `false` (or Dovetail's `DovetailEnableOpenTelemetry`) leaves all of it out. The versions are the
ones LaunchPad pinned when the SDK was packed; set `LaunchPadOpenTelemetryExporterVersion` (and the other `LaunchPad*Version`
properties of these packages) to choose another, or reference the package yourself.

Each technology can be turned off with its `LaunchPadEnable<Technology>` property, which removes its own package and every
integration package it is part of:

```xml
<PropertyGroup>
  <LaunchPadEnableNodaTime>false</LaunchPadEnableNodaTime>
</PropertyGroup>
```

The properties are `LaunchPadEnableHosting`, `LaunchPadEnableFoundation`, `LaunchPadEnableAspNetCore`, `LaunchPadEnableAspNetCoreMvc` (MVC only; it defaults to
`false` when the project sets `PublishAot` or `IsAotCompatible`), `LaunchPadEnableNodaTime`,
`LaunchPadEnableNetTopologySuite`, `LaunchPadEnableZeroAlloc`, `LaunchPadEnableGrpc`, `LaunchPadEnableHotChocolate`,
`LaunchPadEnableStrawberryShake`, `LaunchPadEnableOpenTelemetry` and `LaunchPadEnableOtlpExporter` (the exporter only).

## Direct references

A **direct reference** is a package or project reference a project declares itself, as opposed to one it only receives through
another package or project. The SDK and the [templates](../templates/) act on direct references only: a library the project
receives transitively was chosen by whoever referenced it, not by the project, so it adds nothing to the project.

If a technology arrives only transitively and you want its LaunchPad support, reference it directly.