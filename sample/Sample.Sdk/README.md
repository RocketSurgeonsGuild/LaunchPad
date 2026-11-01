# Rocket.Surgery.LaunchPad.Sdk sample

Consumes LaunchPad purely as an MSBuild SDK from packed NuGet packages, the way an application outside this repository does. No project
here references LaunchPad or Dovetail itself, and nothing is imported from `src/`. The only wiring is one line in
[`Directory.Build.props`](Directory.Build.props):

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk">
```

with the SDK version pinned in [`global.json`](global.json) under `msbuild-sdks`. The LaunchPad SDK chains `Joinery.Dovetail.Sdk`, so
there is no second SDK entry.

| Project              | Shape                   | The SDK should...                                                                                                                       |
| -------------------- | ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `Sample.Sdk.Worker`  | `OutputType=Exe`        | find a generic host, add `Joinery.Dovetail.Hosting` and `Rocket.Surgery.LaunchPad.Hosting`, and write the health check joint            |
| `Sample.Sdk.Web`     | `Microsoft.NET.Sdk.Web` | find an ASP.NET Core host, add the same packages and `Rocket.Surgery.LaunchPad.AspNetCore`; the health checks come with the framework |
| `Sample.Sdk.FileApp` | file-based app          | read its `#:package` directives: a generic host like the worker, plus `Rocket.Surgery.LaunchPad.NodaTime` for its NodaTime reference  |

## Running it

```bash
dotnet msbuild build/pack.proj
dotnet run --project Sample.Sdk.Worker
dotnet run Sample.Sdk.FileApp/app.cs
```

`build/pack.proj` packs the SDK and the packages it adds into `artifacts/nuget-sample` at `0.0.0-sample`, the version `global.json` pins.
Run it again after changing anything under `src/` that those packages are packed from. The worker prints `Health: Healthy`; the web
application serves the same at `/health`; the file-based app prints `Health: Healthy` and `Clock: SystemClock`, the NodaTime clock
LaunchPad.NodaTime registered without the app referencing it.

The file-based app has no project file and is not part of `Sample.Sdk.slnx`: `dotnet run` builds it from the one file, and
`Directory.Build.props` above it applies the SDK exactly as it does to the projects.
