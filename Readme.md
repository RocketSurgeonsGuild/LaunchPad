# LaunchPad

A common set of libraries used to build console and web applications.

LaunchPad 2.0 requires .NET 11 or later: its packages target `net11.0`, and the LaunchPad SDK stops a project on an older .NET with
`LPADSDK0001`.

LaunchPad is built on [Dovetail](https://github.com/RocketSurgeonsGuild/Dovetail), which applies an application's wiring as joints
generated at build time, and [Specular](https://github.com/RocketSurgeonsGuild/Specular), which scans assemblies without reflection.
Health checks, logging, telemetry, validation, JSON and time are joints that Dovetail resolves at build time, without reflection,
so they are trimming and Native AOT safe. Referencing a technology such as NodaTime, HotChocolate, gRPC or ZeroAlloc.Validation is
enough for the LaunchPad SDK to add the matching integration package.

[![github-release-badge]][github-release]
[![github-license-badge]][github-license]
[![github-build-badge]][github-build]
[![codecov-badge]][codecov]

## Getting started

An application uses LaunchPad through a LaunchPad SDK, which takes the place of the Microsoft SDK the project names. In the snippets,
replace `1.0.0` with the version of LaunchPad you use; the SDK is not on nuget.org yet, and the
[`sample/Sample.Sdk`](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/main/sample/Sample.Sdk) applications pack it from the
repository.

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Web/1.0.0">
</Project>
```

The SDK finds the host model and adds the LaunchPad and Dovetail packages that fit it, here `Rocket.Surgery.LaunchPad.Hosting` and
`Rocket.Surgery.LaunchPad.AspNetCore`. Build the host with `ConfigureDovetail()`, which Dovetail generates into the project's root
namespace and which applies every joint the application imports:

```csharp
using Rocket.Surgery.LaunchPad.AspNetCore;

var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
app.MapLaunchPadHealthChecks(); // /health, /health/live and /health/ready
await app.RunAsync();
```

The [getting started guide](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/src/content/docs/guides/index.md) covers
worker, console and file-based applications, and
[choosing integration packages](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/src/content/docs/guides/choosing-packages.md)
lists what the SDK adds for each technology.

## Packages

### SDKs

| Package                              | NuGet             | What it does                                                                                   |
| ------------------------------------ | ----------------- | ---------------------------------------------------------------------------------------------- |
| Rocket.Surgery.LaunchPad.Sdk         | not published yet | MSBuild SDK that chains `Joinery.Dovetail.Sdk` and adds the LaunchPad packages a project needs |
| Rocket.Surgery.LaunchPad.Sdk.Console | not published yet | The same SDK for ConsoleAppFramework applications                                              |
| Rocket.Surgery.LaunchPad.Sdk.Web, .Worker, .Razor, .BlazorWebAssembly, .WebAssembly, .WindowsDesktop | not published yet | The Microsoft SDK of the same name with LaunchPad: a drop-in replacement for it |

### Core

| Package                                                 | NuGet                                         | What it does                                                                                       |
| ------------------------------------------------------- | --------------------------------------------- | -------------------------------------------------------------------------------------------------- |
| [Rocket.Surgery.LaunchPad.Foundation][nuget-foundation] | [![nuget-foundation-badge]][nuget-foundation] | JSON, resilience and service discovery joints, the standard error cases and the JSON context build |
| [Rocket.Surgery.LaunchPad.Analyzers][nuget-analyzers]   | [![nuget-analyzers-badge]][nuget-analyzers]   | The source generators Foundation carries: operation hosts, property tracking, JSON converters      |
| [Rocket.Surgery.LaunchPad.Hosting][nuget-hosting]       | [![nuget-hosting-badge]][nuget-hosting]       | Health check, Serilog and OpenTelemetry joints, written into the application as templates          |
| [Rocket.Surgery.LaunchPad.AspNetCore][nuget-aspnetcore] | [![nuget-aspnetcore-badge]][nuget-aspnetcore] | OpenAPI, problem details, request logging, health check endpoints and System.Text.Json for ASP.NET Core |
| Rocket.Surgery.LaunchPad.AspNetCore.Mvc | not published yet | MVC controllers and Razor Pages: application parts, MVC JSON options, request logging filters |

### Integrations

| Package                                                                           | NuGet                                                                   | What it does                                                          |
| --------------------------------------------------------------------------------- | ----------------------------------------------------------------------- | --------------------------------------------------------------------- |
| [Rocket.Surgery.LaunchPad.Grpc][nuget-grpc]                                       | [![nuget-grpc-badge]][nuget-grpc]                                       | gRPC services on ASP.NET Core                                         |
| [Rocket.Surgery.LaunchPad.HotChocolate][nuget-hotchocolate]                       | [![nuget-hotchocolate-badge]][nuget-hotchocolate]                       | HotChocolate GraphQL servers: error filter, strongly typed id filters |
| [Rocket.Surgery.LaunchPad.StrawberryShake][nuget-strawberryshake]                 | [![nuget-strawberryshake-badge]][nuget-strawberryshake]                 | Scalar serializers for StrawberryShake GraphQL clients                |
| [Rocket.Surgery.LaunchPad.EntityFramework][nuget-entityframework]                 | [![nuget-entityframework-badge]][nuget-entityframework]                 | A base `DbContext` and strongly typed id value generation for EF Core |
| Rocket.Surgery.LaunchPad.NodaTime                                                 | not published yet                                                       | The NodaTime clock, System.Text.Json converters and Serilog policy    |
| Rocket.Surgery.LaunchPad.NodaTime.HotChocolate                                    | not published yet                                                       | NodaTime scalars for HotChocolate                                     |
| Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake                                 | not published yet                                                       | NodaTime scalar serializers for StrawberryShake                       |
| [Rocket.Surgery.LaunchPad.Spatial][nuget-spatial]                                 | [![nuget-spatial-badge]][nuget-spatial]                                 | NetTopologySuite GeoJSON and WKT serialization and a Serilog policy   |
| [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial][nuget-strawberryshake-spatial] | [![nuget-strawberryshake-spatial-badge]][nuget-strawberryshake-spatial] | NetTopologySuite serializers for StrawberryShake                      |
| Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore                                     | not published yet                                                       | ZeroAlloc.Validation failures as 422 problem details and in OpenAPI   |
| Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc                                           | not published yet                                                       | ZeroAlloc.Mediator validation failures as an `InvalidArgument` status |
| Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate                                   | not published yet                                                       | ZeroAlloc.Mediator validation failures as GraphQL errors              |
| Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor                                         | not published yet                                                       | A `ZeroAllocValidator` component for Blazor forms                     |

### Strong types

| Package                                                 | NuGet                                         | What it does                                                                                       |
| ------------------------------------------------------- | --------------------------------------------- | -------------------------------------------------------------------------------------------------- |
| [Rocket.Surgery.LaunchPad.Primitives][nuget-primitives] | [![nuget-primitives-badge]][nuget-primitives] | Ready-made strong types on SuperStrong, with OpenAPI, HotChocolate and EF Core joints as templates |
| Rocket.Surgery.LaunchPad.Primitives.Analyzers           | not published yet                             | The strong type registry generator Primitives depends on                                           |

## Contributing

`mise install` installs the toolchain, `mise run build` builds, tests and packs, and `hk check` lints.
[AGENTS.md](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/AGENTS.md) and
[CONTRIBUTING.md](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/CONTRIBUTING.md) have the repository's conventions.

[github-release]: https://github.com/RocketSurgeonsGuild/LaunchPad/releases
[github-release-badge]: https://img.shields.io/github/v/release/RocketSurgeonsGuild/LaunchPad?include_prereleases&logo=github&style=flat
[github-license]: https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/LICENSE
[github-license-badge]: https://img.shields.io/github/license/RocketSurgeonsGuild/LaunchPad.svg?style=flat
[github-build]: https://github.com/RocketSurgeonsGuild/LaunchPad/actions/workflows/build.yml
[github-build-badge]: https://github.com/RocketSurgeonsGuild/LaunchPad/actions/workflows/build.yml/badge.svg
[codecov]: https://codecov.io/gh/RocketSurgeonsGuild/LaunchPad
[codecov-badge]: https://img.shields.io/codecov/c/github/RocketSurgeonsGuild/LaunchPad.svg?color=E03997&label=codecov&logo=codecov&logoColor=E03997&style=flat
[nuget-analyzers]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Analyzers/
[nuget-analyzers-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.Analyzers.svg?logo=nuget&style=flat
[nuget-aspnetcore]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.AspNetCore/
[nuget-aspnetcore-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.AspNetCore.svg?logo=nuget&style=flat
[nuget-entityframework]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.EntityFramework/
[nuget-entityframework-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.EntityFramework.svg?logo=nuget&style=flat
[nuget-foundation]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Foundation/
[nuget-foundation-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.Foundation.svg?logo=nuget&style=flat
[nuget-grpc]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Grpc/
[nuget-grpc-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.Grpc.svg?logo=nuget&style=flat
[nuget-hosting]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Hosting/
[nuget-hosting-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.Hosting.svg?logo=nuget&style=flat
[nuget-hotchocolate]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.HotChocolate/
[nuget-hotchocolate-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.HotChocolate.svg?logo=nuget&style=flat
[nuget-primitives]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Primitives/
[nuget-primitives-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.Primitives.svg?logo=nuget&style=flat
[nuget-spatial]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Spatial/
[nuget-spatial-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.Spatial.svg?logo=nuget&style=flat
[nuget-strawberryshake]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.StrawberryShake/
[nuget-strawberryshake-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.StrawberryShake.svg?logo=nuget&style=flat
[nuget-strawberryshake-spatial]: https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.StrawberryShake.Spatial/
[nuget-strawberryshake-spatial-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.svg?logo=nuget&style=flat
