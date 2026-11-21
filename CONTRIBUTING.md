# Contributing to LaunchPad

[`AGENTS.md`](AGENTS.md) describes the repository, how to build, test and format it, and the conventions it follows; it applies to
people as much as to agents. This file collects the procedures that touch many places at once.

## Target frameworks

LaunchPad targets .NET 11 and later. The frameworks are declared once, in [`Directory.Build.props`](Directory.Build.props):

| Property                           | Value                                        | Used by                                                                                                                |
| ---------------------------------- | -------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `LaunchPadTargetFrameworks`        | `net11.0`, oldest first                      | every project in `src/`, `sample/` and `test/`, as `<TargetFrameworks>$(LaunchPadTargetFrameworks)</TargetFrameworks>` |
| `LaunchPadBrowserTargetFrameworks` | the same with `-browser` (`net11.0-browser`) | the WebAssembly projects (`sample/Sample.BlazorWasm`, `test/Sample.BlazorWasm.Tests`)                                  |
| `LaunchPadLatestTargetFramework`   | the newest of them                           | the projects that build for one framework: `test/Sdk.Tests` (its host) and `test/Api/Aot`                              |

No project file names a framework itself, except the generators, analyzers, MSBuild task and SDK packages
(`src/Analyzers`, `src/Primitives.Analyzers`, `src/Foundation.Tasks`, the SDKs under `src/Sdks`), which stay on `netstandard2.0`
because the compiler and MSBuild load them, and `sample/Sample.Sdk`, which stands for an application outside the repository.

Code never assumes exactly one framework:

- in C#, `#if NET12_0_OR_GREATER` and the like, never `#if NET11_0`;
- in MSBuild, `$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net12.0'))`, never `'$(TargetFramework)' == 'net11.0'`;
- in tests, read the frameworks from `Directory.Build.props` (`PinnedPackages.TargetFrameworks` in `test/Hosting.Tests/Scaffolding`)
  rather than listing them.

### Adding a target framework

Adding `net12.0` next to `net11.0` touches these places, in this order:

1. **`Directory.Build.props`**: append the framework, `<LaunchPadTargetFrameworks>net11.0;net12.0</LaunchPadTargetFrameworks>`. The
   browser and latest properties follow. Every project, the build's sample publish (`build/Samples.cs`) and the SDK tests'
   `TargetFrameworksAttribute` read it.
2. **The SDK**: `global.json` (and `sample/Sample.Sdk/global.json`) pin an SDK that knows the new framework, and `.config/mise.toml`
   installs it. That SDK only carries the newest runtime, so mise also installs the ASP.NET Core runtime of every older framework
   (`{ version = "11.0.x", runtime = "aspnetcore" }`), and `.github/renovate.json` gets back a custom manager for that runtime entry
   and puts it in the `dotnet monorepo` group. The CI workflows install whatever mise pins and need no change of their own.
3. **`Directory.Packages.props`**: the Microsoft packages that ship once per .NET release train (ASP.NET Core, EF Core,
   `Microsoft.Extensions.*`, `System.Text.Json` and the rest of that group) split into one item group per framework, each
   conditioned with `IsTargetFrameworkCompatible` (`!...('net12.0')` for the 11.0 line, `...('net12.0')` for the 12.0 line).
   `.github/renovate.json` gets a package rule that keeps the older group on its train (`matchCurrentVersion` `^11\.`,
   `allowedVersions` `<12.0.0`). The test scaffolding (`PinnedPackages.Version`) reads such conditions; any other condition fails
   the Hosting and SDK tests until it learns it.
4. **CI**: nothing per framework. Each test project runs once per framework of its `TargetFrameworks`, in the pipeline
   (`build.yml`) and in the package test jobs.
5. **Snapshots**: the tests that keep one snapshot per framework (`UniqueForTargetFrameworkAndVersion`, the samples' OpenAPI
   documents in `test/Sample.Minimal.Tests` and `test/Sample.Restful.Tests`) need a `.DotNet12_0` snapshot. `test/Sdk.Tests` and
   `test/Hosting.Tests` share one snapshot across frameworks, except where the framework is in the result
   (`HostingPackageTests.Should_Depend_On_Dovetail_Only`). Accept with `dotnet verify accept` after reviewing the diff.
6. **Code**: whatever the new framework adds or obsoletes, behind `NET12_0_OR_GREATER`.
7. **Docs**: [`Readme.md`](Readme.md), the guides under `docs/src/content/docs/` and `AGENTS.md` state the minimum; they change only
   when the oldest framework does.

Dropping the oldest framework is the same list in reverse, plus the LaunchPad SDK's minimum
(`LaunchPadMinimumTargetFramework` in `src/Sdks/LaunchPad/Sdk/Rocket.Surgery.LaunchPad.TargetFramework.targets`, tested by
`test/Sdk.Tests/SdkTargetFrameworkTests.cs`), and it is a breaking change for a major release.

## Running the samples together

`sample/Sample.AppHost` is an [Aspire](https://aspire.dev) AppHost that starts the samples side by side, each on ports of its own, with
the Aspire dashboard. From the repository root (`aspire.config.json` points the Aspire CLI at it):

```bash
aspire run
```

The Aspire CLI is in `.config/mise.toml`, at the version of the `Aspire.Hosting.*` packages in `sample/Directory.Packages.props`; update
them together. The AppHost adds the samples by path and Aspire builds each one before it starts it. `Sample.Command` runs a command and
exits, so it starts only when you start it from the dashboard.

The dashboard shows the samples' logs, traces and metrics because Aspire sets `OTEL_EXPORTER_OTLP_ENDPOINT` for them: the samples
reference OpenTelemetry and Serilog's OpenTelemetry sink (`sample/Directory.Build.targets`), and export only when that variable is set.
