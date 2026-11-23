---
title: Templates
description: A template is source shipped inside a package and written into a consuming project only when its host model and references match, so unused wiring never compiles in.
---

A **template** is source shipped inside a package, as a `*.cs.template` file, and written into a consuming project only when that
project's [host model](../host-models/) and references match. Unused wiring never compiles in, and the package that ships the
template depends on none of the libraries the template uses.

`Rocket.Surgery.LaunchPad.Hosting` ships its [Joints](../joints/) this way, and so does `Rocket.Surgery.LaunchPad.Primitives` for the
libraries its [domain primitives](../domain-primitives/) are described to. Each one is a template with three gates, and every gate has to pass:

- **Hosts**: the project declares one of the host models the template is for.
- **References**: every assembly the template compiles against is among the project's references.
- **Direct reference**: the project references the package that ships the template itself (see
  [direct references](../integration-packages/#direct-references)). A project that only receives the package through a library gets
  nothing.

## The Hosting templates

| Joint                                         | Written when the project references                                  | Host models                                    |
| --------------------------------------------- | -------------------------------------------------------------------- | ---------------------------------------------- |
| `LaunchPadHealthChecksJoint`                  | `Microsoft.Extensions.Diagnostics.HealthChecks`                      | generic host, ASP.NET Core                     |
| `LaunchPadSerilogEnrichmentJoint`             | `Joinery.Dovetail.Serilog`                                           | generic host, ASP.NET Core                     |
| `LaunchPadSerilogSystemTextJsonJoint`         | `Joinery.Dovetail.Serilog`                                           | generic host, ASP.NET Core                     |
| `LaunchPadSerilogApplicationStateJoint`       | `Serilog.Extensions.Logging`                                         | generic host, ASP.NET Core                     |
| `LaunchPadSerilogEnvironmentEnrichersJoint`   | the Serilog environment, process, thread and assembly name enrichers | generic host, ASP.NET Core                     |
| `LaunchPadSerilogSpanJoint`                   | `Serilog.Enrichers.Span`                                             | generic host, ASP.NET Core                     |
| `LaunchPadSerilogExceptionsJoint`             | `Serilog.Exceptions` and `Serilog.Enrichers.Demystifier`             | generic host, ASP.NET Core                     |
| `LaunchPadSerilogConsoleJoint`                | `Serilog.Sinks.Console` and `Serilog.Sinks.Async`                    | generic host, ASP.NET Core                     |
| `LaunchPadSerilogDebugJoint`                  | `Serilog.Sinks.Debug` and `Serilog.Sinks.Async`                      | generic host, ASP.NET Core                     |
| `LaunchPadSerilogTraceJoint`                  | `Serilog.Sinks.Trace` and `Serilog.Sinks.Async`                      | generic host, ASP.NET Core                     |
| `LaunchPadSerilogDiagnosticContextJoint`      | `Serilog.Extensions.Hosting`                                         | generic host, ASP.NET Core                     |
| `LaunchPadSerilogOpenTelemetryJoint`          | `Serilog.Sinks.OpenTelemetry`                                        | generic host, ASP.NET Core                     |
| `LaunchPadBrowserConsoleLoggingJoint`         | `Serilog.Sinks.BrowserConsole`                                       | Blazor WebAssembly                             |
| `LaunchPadWebAssemblyEnvironmentLoggingJoint` | `Joinery.Dovetail.Serilog`                                           | Blazor WebAssembly                             |
| `LaunchPadInstrumentationJoint`               | `OpenTelemetry.Instrumentation.Http`                                 | generic host, ASP.NET Core, Blazor WebAssembly |
| `LaunchPadRuntimeInstrumentationJoint`        | `OpenTelemetry.Instrumentation.Runtime`                              | generic host, ASP.NET Core                     |
| `LaunchPadProcessInstrumentationJoint`        | `OpenTelemetry.Instrumentation.Process`                              | generic host, ASP.NET Core                     |
| `LaunchPadAspNetCoreInstrumentationJoint`     | `OpenTelemetry.Instrumentation.AspNetCore`                           | ASP.NET Core                                   |
| `LaunchPadHotChocolateInstrumentationJoint`   | `HotChocolate.Diagnostics`                                           | generic host, ASP.NET Core                     |
| `LaunchPadComponentsInstrumentationJoint`     | `Microsoft.AspNetCore.Components` (ASP.NET Core and Blazor have it)  | ASP.NET Core, Blazor WebAssembly               |
| `LaunchPadOtlpExporterJoint`                  | `OpenTelemetry.Exporter.OpenTelemetryProtocol`                       | generic host, ASP.NET Core                     |
| `LaunchPadWebAssemblyOtlpExporterJoint`       | `OpenTelemetry.Exporter.OpenTelemetryProtocol`                       | Blazor WebAssembly                             |

The Serilog Joints need `Joinery.Dovetail.Serilog` and the OpenTelemetry Joints need `Joinery.Dovetail.OpenTelemetry`, which
Dovetail's SDK adds when the project references Serilog or OpenTelemetry. So choosing a sink or an instrumentation is a matter of
referencing its package: the Joint that turns it on comes with it.

`LaunchPadSerilogOpenTelemetryJoint` (logs) and `LaunchPadOtlpExporterJoint` (traces and metrics) export only when
`OTEL_EXPORTER_OTLP_ENDPOINT` is set, as Aspire sets it for the applications it runs, so an application started on its own doesn't send
to an endpoint nothing listens on. The LaunchPad SDK adds the exporter, and the instrumentation of each host model, to every
application (see [integration packages](../integration-packages/#opentelemetry)).

Blazor WebAssembly has an exporter of its own, `LaunchPadWebAssemblyOtlpExporterJoint`, for its traces, metrics and logs (Serilog's
OpenTelemetry sink doesn't run in the browser, and Dovetail's Serilog logger writes every event to OpenTelemetry's logger provider as
well). A browser can't use OTLP over gRPC, so it always exports HTTP/protobuf, and the endpoint may be a path of the application's own
origin: Aspire's Blazor gateway gives the client the route `/{app}/_otlp`, which it forwards to the dashboard. The gateway's settings
reach the client as environment variables, which a JavaScript initializer of the application sets from the gateway's
`_blazor/_configuration` (see `sample/Sample.BlazorWasm/wwwroot/Sample.BlazorWasm.lib.module.js`); the joint reads each setting from the
configuration first and from the environment after.

## The Primitives templates

| Joint                                     | Written when the project references                                                         | Host models                                    |
| ----------------------------------------- | ------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| `LaunchPadPrimitives` (the registry)      | `Rocket.Surgery.LaunchPad.Primitives`                                                       | generic host, ASP.NET Core, Blazor WebAssembly |
| `LaunchPadPrimitivesOpenApiJoint`         | `Rocket.Surgery.LaunchPad.Primitives` and `Microsoft.AspNetCore.OpenApi`                    | ASP.NET Core                                   |
| `LaunchPadPrimitivesHotChocolateJoint`    | `Rocket.Surgery.LaunchPad.Primitives` and `HotChocolate.Types`                              | generic host, ASP.NET Core                     |
| `LaunchPadPrimitivesEntityFrameworkJoint` | `Rocket.Surgery.LaunchPad.Primitives` and `Microsoft.EntityFrameworkCore`                   | generic host, ASP.NET Core                     |
| `LaunchPadPrimitivesInstrumentationJoint` | `Rocket.Surgery.LaunchPad.Primitives`, `Joinery.Dovetail.OpenTelemetry` and `OpenTelemetry` | generic host, ASP.NET Core, Blazor WebAssembly |

`LaunchPadPrimitives` is the registry the other Joints read, which Primitives' registry generator fills in with every domain primitive
the application declares or references, and its rules. The EF Core Joint configures every `DbContext` with the value converters the
registry generator writes for each domain primitive, maps value objects as complex properties and generates the keys that are strong IDs
of a `Guid`; the instrumentation Joint calls the `AddLaunchPadPrimitivesInstrumentation()` the registry generator writes when the
application references OpenTelemetry. Dapper needs no Joint: the registry generator registers its type handlers in a module initializer.
See [domain primitives](../domain-primitives/#in-openapi-graphql-ef-core-and-dapper).

## Templates or packages

Templates are for gating by host model inside `LaunchPad.Hosting`, where there is no package boundary to draw, and for the Joints
of `LaunchPad.Primitives`, which describe domain primitives to libraries it must not depend on
([ADR 0008](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0008-primitives-is-an-independent-strong-types-package.md),
[ADR 0011](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0011-primitives-is-a-port-of-dotnet-domain-primitives.md)).
Support for a third-party technology ships as an [integration package](../integration-packages/) instead.

The template mechanism itself is Dovetail's:
[Shipping templates](https://rocketsurgeonsguild.github.io/Dovetail/guides/shipping-templates/).
