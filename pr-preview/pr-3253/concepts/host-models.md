# Host models

A **host model** is the kind of application a project is. Dovetail's SDK decides it at build time from the project's SDK, its
`OutputType` and the assemblies it references, and declares it as a `DovetailHost` item. LaunchPad reads that decision to choose
packages and [templates](../templates/); nothing about it is decided at run time.

| Host model        | A project that is…                                                    | `ConfigureDovetail()` is generated for |
| ----------------- | --------------------------------------------------------------------- | -------------------------------------- |
| `HostApplication` | a generic host application: a worker, a console app, a file-based app | `HostApplicationBuilder`               |
| `WebApplication`  | an ASP.NET Core application (`Microsoft.NET.Sdk.Web`)                 | `WebApplicationBuilder`                |
| `WebAssembly`     | a Blazor WebAssembly application                                      | `WebAssemblyHostBuilder`               |
| `Maui`            | a .NET MAUI application (`UseMaui`)                                   | `MauiAppBuilder`                       |
| `Uno`             | an Uno Platform application with the `Hosting` feature                | Uno's `IApplicationBuilder`            |

A library has no host model. It still exports its Joints, and they apply in every application that references it.

## What the host model decides

- **Packages.** The [LaunchPad SDK](../launchpad-sdk/) adds `Rocket.Surgery.LaunchPad.Hosting` to any project with a host model,
  and `Rocket.Surgery.LaunchPad.AspNetCore` to a `WebApplication`.
- **Templates.** Each template names the host models it is for. The Serilog console sink Joint, for example, is written into
  generic host and ASP.NET Core applications, and the browser console Joint only into Blazor WebAssembly ones.
- **The entry point.** `ConfigureDovetail()` is generated for the host builder of the project's host model.

Dovetail's [host reference](https://rocketsurgeonsguild.github.io/Dovetail/reference/dovetail-host/) describes how each one is
detected and how to declare one yourself.