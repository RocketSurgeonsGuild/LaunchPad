---
title: Templates
description: A template is source shipped inside a package and written into a consuming project only when its host model and references match, so unused wiring never compiles in.
---

A **template** is source shipped inside a package, as a `*.cs.template` file, and written into a consuming project only when that
project's [host model](../host-models/) and references match. Unused wiring never compiles in, and the package that ships the
template depends on none of the libraries the template uses.

`Rocket.Surgery.LaunchPad.Hosting` ships its [Joints](../joints/) this way. Each one is a template with three gates, and every gate
has to pass:

- **Hosts**: the project declares one of the host models the template is for.
- **References**: every assembly the template compiles against is among the project's references.
- **Direct reference**: the project references `Rocket.Surgery.LaunchPad.Hosting` itself (see
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

The Serilog Joints need `Joinery.Dovetail.Serilog` and the OpenTelemetry Joints need `Joinery.Dovetail.OpenTelemetry`, which
Dovetail's SDK adds when the project references Serilog or OpenTelemetry. So choosing a sink or an instrumentation is a matter of
referencing its package: the Joint that turns it on comes with it.

## Templates or packages

Templates are reserved for gating by host model inside `LaunchPad.Hosting`, where there is no package boundary to draw. Support
for a third-party technology ships as an [integration package](../integration-packages/) instead.

The template mechanism itself is Dovetail's:
[Shipping templates](https://rocketsurgeonsguild.github.io/Dovetail/guides/shipping-templates/).
