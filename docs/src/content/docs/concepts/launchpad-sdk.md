---
title: LaunchPad SDK
description: The MSBuild SDK an application opts into so that the right LaunchPad packages are chosen from what the project references. It builds on Dovetail's SDK.
---

The **LaunchPad SDK**, `Rocket.Surgery.LaunchPad.Sdk`, is an MSBuild SDK an application opts into so that the right LaunchPad
packages are chosen from what the project references. It is the only LaunchPad entry the application needs: no
`PackageReference` to LaunchPad or Dovetail.

## How it builds on Dovetail's SDK

The LaunchPad SDK chains `Joinery.Dovetail.Sdk`, so there is no second SDK entry. When a project builds:

1. Dovetail's SDK decides the project's [host model](../host-models/) and adds `Joinery.Dovetail.Hosting`, plus
   `Joinery.Dovetail.Serilog` and `Joinery.Dovetail.OpenTelemetry` when the project references Serilog or OpenTelemetry. LaunchPad
   turns Serilog on by default.
2. The LaunchPad SDK reads that host model and the project's [direct references](../integration-packages/#direct-references), and
   adds the [integration packages](../integration-packages/#the-packages-the-sdk-chooses) that match.
3. NuGet restores everything both SDKs added, so the packages behave like ordinary references. A package the project already
   references itself is left alone.

All of it happens while MSBuild evaluates the project, before restore. Nothing is decided at run time.

## The SDKs

LaunchPad has an SDK for each SDK Dovetail ships. Each chains its Dovetail counterpart and adds what
`Rocket.Surgery.LaunchPad.Sdk` adds, so a project gets LaunchPad by changing its SDK name and nothing else:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">                   <!-- before -->
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Web/1.0.0">  <!-- after -->
```

| Microsoft SDK                         | Dovetail SDK                             | LaunchPad SDK                                    |
| ------------------------------------- | ---------------------------------------- | ------------------------------------------------ |
| `Microsoft.NET.Sdk`                   | `Joinery.Dovetail.Sdk`                   | `Rocket.Surgery.LaunchPad.Sdk`                   |
| (console applications)                | `Joinery.Dovetail.Sdk.Console`           | `Rocket.Surgery.LaunchPad.Sdk.Console`           |
| `Microsoft.NET.Sdk.Web`               | `Joinery.Dovetail.Sdk.Web`               | `Rocket.Surgery.LaunchPad.Sdk.Web`               |
| `Microsoft.NET.Sdk.Worker`            | `Joinery.Dovetail.Sdk.Worker`            | `Rocket.Surgery.LaunchPad.Sdk.Worker`            |
| `Microsoft.NET.Sdk.Razor`             | `Joinery.Dovetail.Sdk.Razor`             | `Rocket.Surgery.LaunchPad.Sdk.Razor`             |
| `Microsoft.NET.Sdk.BlazorWebAssembly` | `Joinery.Dovetail.Sdk.BlazorWebAssembly` | `Rocket.Surgery.LaunchPad.Sdk.BlazorWebAssembly` |
| `Microsoft.NET.Sdk.WebAssembly`       | `Joinery.Dovetail.Sdk.WebAssembly`       | `Rocket.Surgery.LaunchPad.Sdk.WebAssembly`       |
| `Microsoft.NET.Sdk.WindowsDesktop`    | `Joinery.Dovetail.Sdk.WindowsDesktop`    | `Rocket.Surgery.LaunchPad.Sdk.WindowsDesktop`    |

`Rocket.Surgery.LaunchPad.Sdk.Web` and the others below it are **mirrors**: each imports Dovetail's mirror of the Microsoft
SDK, which imports the Microsoft SDK and chains `Joinery.Dovetail.Sdk`, and then adds LaunchPad. A project whose only SDK is a
mirror gets exactly what the Microsoft SDK with `<Sdk Name="Rocket.Surgery.LaunchPad.Sdk" />` next to it gets.

### Which SDK do I use

- **An ASP.NET Core application** (`Microsoft.NET.Sdk.Web` today): `Rocket.Surgery.LaunchPad.Sdk.Web`. It gets
  `Rocket.Surgery.LaunchPad.Hosting`, `Rocket.Surgery.LaunchPad.AspNetCore` and `Rocket.Surgery.LaunchPad.AspNetCore.Mvc` (MVC
  controllers and Razor Pages). A project that sets `PublishAot` or `IsAotCompatible` to `true` doesn't get
  `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`, which isn't native AOT compatible (see [native AOT](#native-aot)).
- **A worker service** (`Microsoft.NET.Sdk.Worker`): `Rocket.Surgery.LaunchPad.Sdk.Worker`. It gets
  `Rocket.Surgery.LaunchPad.Hosting`.
- **A Blazor WebAssembly application** (`Microsoft.NET.Sdk.BlazorWebAssembly`), or a WebAssembly application from the
  `wasmbrowser` template (`Microsoft.NET.Sdk.WebAssembly`): `Rocket.Surgery.LaunchPad.Sdk.BlazorWebAssembly` or
  `Rocket.Surgery.LaunchPad.Sdk.WebAssembly`. It gets `Rocket.Surgery.LaunchPad.Hosting`.
- **A command line application**: `Rocket.Surgery.LaunchPad.Sdk.Console` (see [below](#the-console-sdk)).
- **A library, or a console application on the generic host** (`Microsoft.NET.Sdk`), or a file-based app:
  `Rocket.Surgery.LaunchPad.Sdk`.
- **A Razor class library** (`Microsoft.NET.Sdk.Razor`): `Rocket.Surgery.LaunchPad.Sdk.Razor`. Dovetail takes it for an
  ASP.NET Core host, but a class library is not an application, so LaunchPad adds none of its hosting packages; the
  packages that follow from its references (NodaTime, ...) still apply.
- **An Aspire AppHost**: `Rocket.Surgery.LaunchPad.Sdk`, with the `Aspire.Hosting.AppHost` and `Aspire.Hosting.Dotnet` packages (see
  [below](#aspire-apphosts)). Dovetail takes an AppHost for a generic host, but it starts the application's projects rather than hosting
  LaunchPad's joints, so LaunchPad adds none of its hosting packages.
- **A Windows Forms or WPF project that still names `Microsoft.NET.Sdk.WindowsDesktop`**: `Rocket.Surgery.LaunchPad.Sdk.WindowsDesktop`.
  That SDK is obsolete since .NET 5; a project that has moved to `Microsoft.NET.Sdk` uses `Rocket.Surgery.LaunchPad.Sdk`.

A Razor class library or an AppHost that wants LaunchPad's hosting packages after all sets `LaunchPadEnableHosting` (and, for
ASP.NET Core, `LaunchPadEnableAspNetCore`) to `true`.

### Aspire AppHosts

There is no Aspire SDK to mirror any more. Since Aspire 17 a C# AppHost is `Microsoft.NET.Sdk` with the `Aspire.Hosting.AppHost` and
`Aspire.Hosting.Dotnet` packages, and `Aspire.AppHost.Sdk` is obsolete (`ASPIRE012`). So an AppHost swaps `Microsoft.NET.Sdk` for
`Rocket.Surgery.LaunchPad.Sdk`, like any other project, and keeps its Aspire packages:

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk/1.0.0">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="17.0.0" />
    <PackageReference Include="Aspire.Hosting.Dotnet" Version="17.0.0" />
  </ItemGroup>
</Project>
```

`Aspire.Hosting.AppHost` sets `IsAspireHost`, which is how LaunchPad knows the project is an AppHost. Add .NET projects as resources with
`AddDotnetProject`: without `Aspire.AppHost.Sdk`, a project reference no longer generates `Projects.*` metadata. Builds resolve the Aspire
CLI (`ASPIRE009` when they can't), so install it alongside, at the same version as the packages.

`Rocket.Surgery.LaunchPad.Sdk.Aspire.AppHost`, the mirror of `Aspire.AppHost.Sdk`, is no longer published. A project that used it
switches to `Rocket.Surgery.LaunchPad.Sdk` as above.

### Native AOT

Every package the SDKs add is native AOT compatible, except the ones LaunchPad splits out for what isn't: today that's
`Rocket.Surgery.LaunchPad.AspNetCore.Mvc`, because ASP.NET Core doesn't support MVC with native AOT. A project that sets
`PublishAot` or `IsAotCompatible` to `true` gets only the compatible packages. `LaunchPadEnableAspNetCoreMvc` overrides that either
way: `true` adds MVC to a native AOT project, `false` leaves it out of any other.

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Web">
  <PropertyGroup>
    <PublishAot>true</PublishAot>
  </PropertyGroup>
</Project>
```

The packages that follow from a library the project references, such as `Rocket.Surgery.LaunchPad.HotChocolate` for Hot Chocolate,
are added either way: the project chose that library. [JSON and native AOT](../../guides/json-and-native-aot/#package-compatibility)
lists which packages are compatible.

## Opting in

Make a LaunchPad SDK the project's only SDK, as above, or name `Rocket.Surgery.LaunchPad.Sdk` next to the one the project
already uses:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <Sdk Name="Rocket.Surgery.LaunchPad.Sdk" Version="1.0.0" />
</Project>
```

or make `Rocket.Surgery.LaunchPad.Sdk` itself the project's SDK (it brings `Microsoft.NET.Sdk` through Dovetail's):

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk/1.0.0">
```

or apply it to every project in a `Directory.Build.props`, with the version pinned in `global.json`:

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk">
```

```json
{
    "msbuild-sdks": {
        "Rocket.Surgery.LaunchPad.Sdk": "1.0.0"
    }
}
```

A mirror under a `Directory.Build.props` that already applies `Rocket.Surgery.LaunchPad.Sdk` (or `Joinery.Dovetail.Sdk`) does
not apply it a second time: a repository can apply the SDK to every project and still name the mirror in its web projects.

A file-based app uses the directive form, `#:sdk Rocket.Surgery.LaunchPad.Sdk@1.0.0`, and gets what a console application needs
without a `#:package` directive for LaunchPad.

[Getting started](../../guides/) walks through each of these.

## Target frameworks

LaunchPad requires .NET 11 or later. A project that applies the SDK and targets an older .NET or .NET Framework stops before
restore with `LPADSDK0001`, which names the minimum (`net11.0`), instead of NuGet reporting every LaunchPad package as
incompatible. The check compares versions, so later frameworks pass, and it runs per framework, so a project that targets several
fails on the old one. A .NET Standard library is left alone: the SDK adds it nothing.

## The console SDK

`Rocket.Surgery.LaunchPad.Sdk.Console` is the same SDK for command line applications: it adds the same LaunchPad packages and
chains `Joinery.Dovetail.Sdk.Console` instead of `Joinery.Dovetail.Sdk`, which makes the project an `Exe`, adds ConsoleAppFramework
and writes `Program.cs`. See [Console applications](../../guides/console-apps/).

## Choosing another Dovetail version

Each SDK chains the Dovetail SDK version it was built with. To chain another, set `LaunchPadDovetailSdkVersion` before the SDK is
imported: as a global property (`dotnet build -p:LaunchPadDovetailSdkVersion=...`) or an environment variable, or, for
`Rocket.Surgery.LaunchPad.Sdk` and `Rocket.Surgery.LaunchPad.Sdk.Console`, in a `Directory.Build.props` that sets it and then imports
the SDK with `<Import Project="Sdk.props" Sdk="Rocket.Surgery.LaunchPad.Sdk" />`. A mirror reads it before the Microsoft SDK imports
the `Directory.Build.props`, so for a mirror only the global property or the environment variable works.
