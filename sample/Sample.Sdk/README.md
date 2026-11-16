# Rocket.Surgery.LaunchPad.Sdk sample

Consumes LaunchPad purely as an MSBuild SDK from packed NuGet packages, the way an application outside this repository does. No project
here references LaunchPad or Dovetail itself, and nothing is imported from `src/`. The only wiring is each project's SDK, a LaunchPad SDK
in place of the Microsoft SDK it would otherwise name:

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Web/0.0.0-sample">
```

and the `#:sdk Rocket.Surgery.LaunchPad.Sdk` directive in the file-based app. `Rocket.Surgery.LaunchPad.Sdk.Web` is `Microsoft.NET.Sdk.Web`
with LaunchPad (`Sample.Sdk.Worker` uses `Rocket.Surgery.LaunchPad.Sdk.Worker` the same way): it chains Dovetail's `Joinery.Dovetail.Sdk.Web`,
so there is no second SDK entry.
`Sample.Sdk.Console` shows the project SDK syntax instead, with the console SDK: `Rocket.Surgery.LaunchPad.Sdk.Console` is the project's only
SDK, named with its version. It chains `Joinery.Dovetail.Sdk.Console`, which brings `Microsoft.NET.Sdk`, makes the project an Exe, adds
ConsoleAppFramework and writes `Program.cs`; the project has only its commands and the joint that registers them:

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Console/0.0.0-sample">
```

It could equally be applied once for every project, in a `Directory.Build.props` (`<Project Sdk="Rocket.Surgery.LaunchPad.Sdk">`, with its
version under `msbuild-sdks` in `global.json`).

| Project              | Shape                   | The SDK should...                                                                                                                       |
| -------------------- | ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `Sample.Sdk.Worker`  | `Sdk="Rocket.Surgery.LaunchPad.Sdk.Worker/…"` | find a generic host, add `Joinery.Dovetail.Hosting` and `Rocket.Surgery.LaunchPad.Hosting`, and write the health check joint            |
| `Sample.Sdk.Web`     | `Sdk="Rocket.Surgery.LaunchPad.Sdk.Web/…"` | find an ASP.NET Core host, add the same packages and `Rocket.Surgery.LaunchPad.AspNetCore`; the health checks come with the framework |
| `Sample.Sdk.Console` | `Sdk="Rocket.Surgery.LaunchPad.Sdk.Console/…"` | bring `Microsoft.NET.Sdk` and ConsoleAppFramework, write `Program.cs`, then the same as `Sample.Sdk.Worker` |
| `Sample.Sdk.FileApp` | file-based app          | add what a console application needs with no `#:package` for it: the generic host, `Rocket.Surgery.LaunchPad.Foundation`, `Hosting` and `NodaTime`, and Dovetail's Serilog and OpenTelemetry packages |

## Running it

```bash
dotnet msbuild build/pack.proj
dotnet run --project Sample.Sdk.Worker
dotnet run --project Sample.Sdk.Console
dotnet run Sample.Sdk.FileApp/app.cs
```

`build/pack.proj` packs the four SDKs the sample uses and the packages they add into `artifacts/nuget-sample` at `0.0.0-sample`, the version the projects name.
Run it again after changing anything under `src/` that those packages are packed from, and after changing the SDK itself also run
`dotnet build-server shutdown`: MSBuild nodes that are still running keep the SDK files they read before. The worker and the console app (its root command) print `Health: Healthy`; the web
application serves the same at `/health`; the file-based app prints `Health: Healthy` and `Clock: SystemClock`, the NodaTime clock
LaunchPad.NodaTime registered without the app referencing it or NodaTime.

The file-based app has no project file and is not part of `Sample.Sdk.slnx`: `dotnet run` builds it from the one file. Its one `#:sdk`
directive is enough: the SDK brings `Microsoft.NET.Sdk`, which the directive replaces, and `Directory.Build.props` above it gives it the
projects' shared settings.
