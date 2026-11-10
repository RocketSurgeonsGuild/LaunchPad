# Templates

A **template** is source shipped inside a package, as a `*.cs.template` file, and written into a consuming project only when that
project's [host model](../host-models/) and references match. Unused wiring never compiles in, and the package that ships the
template depends on none of the libraries the template uses.

`Rocket.Surgery.LaunchPad.Hosting` ships its [Joints](../joints/) this way, and so does `Rocket.Surgery.LaunchPad.Primitives` for the
libraries it supports. Each one is a template with three gates, and every gate has to pass:

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

The Serilog Joints need `Joinery.Dovetail.Serilog` and the OpenTelemetry Joints need `Joinery.Dovetail.OpenTelemetry`, which
Dovetail's SDK adds when the project references Serilog or OpenTelemetry. So choosing a sink or an instrumentation is a matter of
referencing its package: the Joint that turns it on comes with it.

## The Primitives templates

| Joint                                   | Written when the project references                                              | Host models  |
| --------------------------------------- | -------------------------------------------------------------------------------- | ------------ |
| `LaunchPadDomainPrimitivesOpenApiJoint` | `EricksonLopez.DomainPrimitives.Abstractions` and `Microsoft.AspNetCore.OpenApi` | ASP.NET Core |

`LaunchPadDomainPrimitivesOpenApiJoint` describes the domain primitives of
[EricksonLopez.DomainPrimitives](https://github.com/ericksonlopezf/dotnet-domain-primitives) in every OpenAPI document as the values
they are written as. A primitive gets the schema of the value of the `IDomainPrimitive<TSelf, TValue>` it implements, narrowed by
its attributes (`[Email]` and `[Url]` give a format, `[Length]`, `[MinLength]`, `[MaxLength]` and `[ExactLength]` a length,
`[Regex]` a pattern, `[PrimitiveRange]` a minimum and maximum), in a body and in a route, query or header parameter, from a
Minimal API or an MVC controller. Without it ASP.NET Core describes a primitive as an empty schema in a body and as a string in a
parameter. The library's own OpenAPI package is a filter for Swashbuckle.

The SDK gives a project that references any of the library's packages a direct reference to `Rocket.Surgery.LaunchPad.Primitives`.
An application whose primitives come from another project references the library (or its ASP.NET Core package) itself to get the
Joint. The package's targets also run the library's source generator, which version 3.0.0 of
`EricksonLopez.DomainPrimitives.Generators` packs where NuGet doesn't run it; `LaunchPadRunDomainPrimitivesGenerator` set to `false`
turns that off.

## Templates or packages

Templates are for gating by host model inside `LaunchPad.Hosting`, where there is no package boundary to draw, and for the glue
between a third-party library and the libraries LaunchPad already builds on, which `LaunchPad.Primitives` ships so that no package
has to depend on the third-party library. Support for a third-party technology that needs code of its own ships as an
[integration package](../integration-packages/).

The template mechanism itself is Dovetail's:
[Shipping templates](https://rocketsurgeonsguild.github.io/Dovetail/guides/shipping-templates/).