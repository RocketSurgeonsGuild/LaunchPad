# Rebuild LaunchPad on Specular and Dovetail

Status: **draft for approval**. No GitHub issues exist yet. Branch: `feature/dovetail-migration`.

## Goal

LaunchPad becomes a simple starting point for building .NET applications, assembled from two building blocks: **Specular** (AOT-safe assembly scanning) and **Dovetail** (build-time composition with Joints). LaunchPad adds opinionated wiring on top, split into small packages that an MSBuild SDK selects from what a project references.

Vocabulary lives in [GLOSSARY.md](../GLOSSARY.md). The two hard-to-reverse decisions are recorded in [docs/adr](../docs/adr).

## Decisions

| #   | Decision                                                                                                                                                                                           |
| --- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1   | Keep the `Rocket.Surgery.LaunchPad.*` package IDs and namespaces. Every project, test and sample targets `net10.0;net11.0` (`net10.0-browser;net11.0-browser` for WebAssembly) on SDK 11 RC.       |
| 2   | Every convention becomes a Dovetail **Joint**. `Rocket.Surgery.Conventions*` is removed, `ConfigureDovetail()` replaces `ConfigureRocketSurgery()`, and scanning uses `ISpecularProvider` (ADR 1). |
| 3   | Keep every current feature area. Remove only what is named below plus dead code.                                                                                                                   |
| 4   | "Environment" means the **host model**, decided at build time. Hosting templates are emitted only into projects with a **direct reference** to `LaunchPad.Hosting`.                                |
| 5   | `AssemblyInfo` (MSBuildSdk) gains typed `AssemblyInfo.Dependencies.<Assembly>` copies, found through a marker the producing assembly stamps. `LaunchPad.Metadata` is dropped.                      |
| 6   | `Rocket.Surgery.LaunchPad.Sdk` chains `Joinery.Dovetail.Sdk`, detects **direct** `PackageReference`s, and adds integration packages by default (`LaunchPadEnable<Tech>=false` opts out).           |
| 7   | Application lifecycle hooks move into Dovetail. Handlers keep running in parallel, as today.                                                                                                       |
| 8   | Serilog mechanism lives in `Joinery.Dovetail.Serilog`; opinionated joints live in `LaunchPad.Hosting`. `LaunchPad.Serilog` is deleted.                                                             |
| 9   | One package per third-party integration (ADR 2). NodaTime and FluentValidation are extracted; Newtonsoft.Json and NetTopologySuite keep their split.                                               |
| 10  | The two Testing packages become non-packable test support. All tests convert to TUnit on Microsoft.Testing.Platform.                                                                               |
| 11  | `IExecuteScoped<T>` and `AddExecuteScopedServices()` are dropped; their users create scopes directly.                                                                                              |
| 12  | `LaunchPad.WebAssembly.Hosting` folds into `LaunchPad.Hosting`.                                                                                                                                    |
| 13  | "Joint" replaces "convention" in code and docs. `InstrumentationConvention` becomes `InstrumentationJoint`.                                                                                        |
| 14  | `Sample.Pages` is restored. `Sample.Command` is rebuilt on Spectre.Console.Cli with `Joinery.Dovetail.DryIoc`.                                                                                     |
| 15  | `SerilogTelemetryConvention` (never exported today) is ported as a live joint.                                                                                                                     |

## Sequence

| Phase | Name                   | Repos                | Issues              |
| ----- | ---------------------- | -------------------- | ------------------- |
| 0     | Tooling and docs       | LaunchPad            | done on this branch |
| 1     | Green baseline         | LaunchPad            | L1, L2, L3, L4      |
| 2     | Upstream (parallel)    | Dovetail, MSBuildSdk | D1-D5, M1, M2       |
| 3     | Restructure            | LaunchPad            | L5-L14              |
| 4     | LaunchPad SDK          | LaunchPad            | L15                 |
| 5     | Docs, tooling, release | LaunchPad            | L16-L19             |

Phase 2 runs alongside Phase 1. Upstream issues are filed in their own repositories and marked as blocking the LaunchPad issues that need them.

### Blocking relationships

| Issue | Blocked by        |
| ----- | ----------------- |
| L5    | L1, L2, L3, L4    |
| L7    | M1                |
| L8    | L5, D1, D4        |
| L9    | L8, D3            |
| L10   | L8, D2            |
| L11   | L5, L10           |
| L12   | L5                |
| L13   | L5, L10           |
| L14   | L5, L8            |
| L15   | L8, L11, L12, L13 |
| L16   | L15               |
| L18   | L1-L15            |

## Epic: Rebuild LaunchPad on Specular and Dovetail

Umbrella issue in `RocketSurgeonsGuild/LaunchPad`. Sub-issues L1-L19. Links to D1-D5 and M1-M2 as blockers.

---

## Upstream: Dovetail (`RocketSurgeonsGuild/Dovetail`)

### D1. Application lifecycle hooks in `Joinery.Dovetail.Hosting`

Labels: `:rocket: feature`

Dovetail has no support for `IHostedLifecycleService` or `IHostApplicationLifetime`. LaunchPad has this in `src/Hosting/ApplicationLifecycleExtensions.cs`.

- Move the 12 extension methods (`OnHostStarting`, `OnHostStarted`, `OnHostStopping`, `OnHostStopped`, each with `Action<IServiceProvider>`, `Func<IServiceProvider, Task>` and `Func<IServiceProvider, CancellationToken, Task>` overloads) on `IHostApplicationBuilder`.
- Move the internal `IHostedLifecycleService` that runs registered handlers. Keep today's behaviour: handlers for a phase run in parallel (`Task.WhenAll`).
- Add `IServiceCollection` overloads so hosts built with `IHostBuilder` work too.
- Register the service automatically from the generated hosting helper wherever `Microsoft.Extensions.Hosting` is referenced, gated like `DovetailHostApplicationHelpers`. Hosts without `IHostedLifecycleService` (MAUI, Blazor WebAssembly) get nothing.
- Cover it in `Joinery.Dovetail.Hosting.Tests` and `Sdk.Tests/HostingHelperTests`; add a docs page.

Acceptance: a generic host and an ASP.NET Core host run starting, started, stopping and stopped handlers; the test that today lives in `Sample.Pages.Tests` is ported.

### D2. `Joinery.Dovetail.Serilog` builds and registers the logger

Labels: `:rocket: feature`

Today `SerilogJoint` builds a `LoggerConfiguration` and discards it. Nothing calls `CreateLogger`, `AddSerilog` or `ReadFrom.Configuration`, the package configures no sinks or enrichers, and no test project covers the runtime.

- Create the `Logger` from the `LoggerConfiguration` after all `ISerilogJoint`s have run.
- Register `Logger`, `ILogger` and `ILoggerFactory` (`SerilogLoggerFactory`) with a `LoggerProviderCollection`, removing the default `Microsoft.Extensions.Logging` provider registrations (as `LaunchPad.Serilog`'s `LoggerConvention` does).
- Apply DI-registered `IConfigureOptions<LoggerConfiguration>` and `IPostConfigureOptions<LoggerConfiguration>`.
- Read the `Serilog` configuration section with `Serilog.Settings.Configuration`. Replace or remove the hand-written `SerilogRuntimeConfiguration` stand-in.
- Add runtime tests and a docs page. Note the AOT and trimming implications of `ReadFrom.Configuration`.

Acceptance: an app that references Serilog and applies one joint gets a working `ILogger<T>` backed by Serilog, configured from `appsettings`.

### D3. Verify `Joinery.Dovetail.OpenTelemetry` covers `LaunchPad.Telemetry`

Labels: `:hammer: chore`

LaunchPad.Telemetry exposes six convention interfaces (base, async, logging, metrics, resource, tracing). Dovetail already has the base joint plus `ITracerProviderJoint`, `IMeterProviderBuilderJoint`, `ILoggerProviderBuilderJoint` and `IResourceBuilderJoint`, and generates async variants. No functional gap is expected.

- Confirm parity, including log export through the logging provider and behaviour on Blazor WebAssembly, MAUI and Uno.
- Confirm the SDK's default resource already includes what `DefaultTelemetryConvention` adds (`AddTelemetrySdk`, environment-variable detector). If not, add the defaults here.
- Fix anything found; otherwise close with a short note in the docs.

### D4. Spike: supported extension point for gated templates

Labels: `:fire: enhancement`

`Joinery.Dovetail.Hosting` gates its templates on referenced assemblies through underscore-prefixed items (`_DovetailHostingHelper`, `References` metadata). `LaunchPad.Hosting` needs the same thing, plus a **direct reference only** mode.

- Decide whether Dovetail exposes a supported public item (for example a `DovetailTemplate` item with `References` and host-model metadata) that downstream packages declare in their own `build/*.targets`.
- If yes, implement it, document it, and keep `Joinery.Dovetail.Hosting` as its first user.
- If no, record that LaunchPad reimplements the gating targets, and why.

Blocks L8. Fallback is LaunchPad-owned targets.

### D5. Honour `[LiveJoint]`, `[UnitTestJoint]` and `[DovetailHostType]` in generated metadata

Labels: `:lady_beetle: bug` (verify first)

`ExportDovetails.HandleDovetailExports` appears to check only `IsTestProject`, so generated export metadata ignores those attributes; only the runtime reflection path in `DovetailContextBuilder.FromDovetail` honours them. LaunchPad relies on live-only and unit-test-only wiring (`SerilogReadFromConfigurationConvention` is live-only; `FakeTimeConvention` is unit-test-only).

- Reproduce with a test, then fix or document.

---

## Upstream: MSBuildSdk (`RocketSurgeonsGuild/MSBuildSdk`)

### M1. `AssemblyInfo.Dependencies`: typed info for every dependent assembly

Labels: `:rocket: feature`

`Rocket.Surgery.AssemblyInfo` generates one `partial class AssemblyInfo` per assembly (Attributes, Metadata, Project, Git, Constants, Resources, Strings) and reads nothing from referenced assemblies. `InternalsVisibleTo` cannot solve this generically because a dependency would have to name every consumer.

- A producing assembly stamps a marker (the spike chooses the exact form).
- The consuming project's generator enumerates referenced assemblies (transitive ones included), finds the marked ones, and emits typed `AssemblyInfo.Dependencies.<AssemblyName>` copies with the same shape. No runtime reflection, no `InternalsVisibleTo`.
- Opt-in through an MSBuild property (for example `AssemblyInfoIncludeDependencies=true`).
- Spike question: read the producer's generated constants through Roslyn symbols, or embed the data in the marker. Symbols cover constants but not the `public` mode, which emits properties.
- Handle name sanitising and collisions; keep the Roslyn 5.0, 5.3, 5.6 and 5.9 builds in step; extend the test harness to compile multiple assemblies; add Verify snapshots and an SDK end-to-end test.

Blocks L7. Replaces the typed `GitVersion` view in `LaunchPad.Metadata`.

### M2. AssemblyInfo hygiene (verify each before fixing)

Labels: `:hammer: chore`

Found while reading; none confirmed by running.

- `build/AssemblyInfo.Constants.targets` replaces newlines with a backslash-n sequence while `ConstantsGenerator.cs` reverts a backtick-n sequence, so multiline constants may not round-trip.
- Resx files with any dot in the file name are skipped silently, not only culture variants.
- Mixed named and indexed format strings generate nothing and report no diagnostic.
- No tests for `AssemblyInfoVisibility=public` or `AssemblyInfoGitComment`.
- The `Rocket.Surgery.Sdk.Script` project comment names a `.nuspec` that does not exist.

---

## LaunchPad (`RocketSurgeonsGuild/LaunchPad`)

### Phase 1: green baseline

#### L1. Restore and build on `net10.0;net11.0`

Labels: `:hammer: chore`

LaunchPad does not build today.

- `LaunchPad.slnx` lists deleted paths: `.build/.build.csproj`, `Directory.Packages.support.props`, `ci-ignore.yml`, `publish-nuget.yml`, `.husky/*`. Fix it and keep `Sample.Pages`.
- `Primitives.csproj` references `Specular` but `Directory.Packages.props` has no `Specular` version.
- Multi-target every project `net10.0;net11.0` (`net10.0-browser;net11.0-browser` for WebAssembly; analyzers stay `netstandard2.0`).
- Use TFM-conditional package versions: `Microsoft.*` stays on 10.0.x for net10 and 11.0.x for net11, as Dovetail does. List any dependency without net11 assets.

Acceptance: `mise run build` compiles every project for both TFMs.

#### L2. Scan with Specular; drop `IExecuteScoped<T>`

Labels: `:hammer: chore`

The working tree calls Indago-era APIs that Specular `main` does not have: `GetCompiledTypeProvider()`, `AddExecuteScopedServices()`, `IExecuteScoped<T>`, and `using Indago`. Specular `main` has `ISpecularProvider`, `AddSpecularServiceRegistrations`, `TypeInfoFilter` and `TypeKindFilter`.

- Move `AspNetCoreConvention`, `MediatRConvention`, `FluentValidationConvention` and `DefaultConvention` to the Specular API.
- Remove `IExecuteScoped<T>` and `AddExecuteScopedServices()`. Rewrite `Sample.Worker`'s `BackgroundWorker` and `DefaultConvention` to create scopes directly.
- Design point: how a convention (later a joint) obtains its `ISpecularProvider`.

#### L3. Convert tests from xunit v3 to TUnit

Labels: `:hammer: chore`

309 tests (198 `[Fact]`, 111 `[Theory]`) across `test/*`. The working tree already swapped package references to TUnit but the tests are still xunit.

- Move to TUnit on Microsoft.Testing.Platform with `AutoFakeTest<TUnitTestRecord>`, `Verify.TUnit`, FakeItEasy and Shouldly, as in Dovetail, Specular and Testing.
- Replace `Xunit.Combinatorial` with TUnit data sources.
- Restore `Sample.Pages.Tests` (and `Sample.Pages` itself, 61 files, restored on this branch from git).

Acceptance: `dotnet test` is green on both TFMs; Verify snapshots differ only by file extension if at all.

#### L4. Replace the Testing packages with non-packable test support

Labels: `:hammer: chore`

- Create one non-packable project under `test/` holding `FakeTimeConvention` (from `src/Testing`; used by 6 test projects) and the Alba fixtures (`LaunchPadWebAppFixture<TEntryPoint>`, `ILaunchPadWebAppFixture`, `IResettableAlbaExtension`, `LaunchPadExtension`; from `src/AspNetCore.Testing`, used by 7 sample test projects).
- Delete `src/Testing` and `src/AspNetCore.Testing`. Nothing from them ships to NuGet.
- The redesign you plan for later stays open.

### Phase 3: restructure

#### L5. Adopt Dovetail and convert conventions to joints

Labels: `:boom: breaking change`

Epic-sized; split per project when scheduled (Primitives, Foundation, AspNetCore, Grpc, HotChocolate, StrawberryShake, Spatial, Mapping, EntityFramework, and so on).

- `[ExportConvention]` types become `[DovetailExport]` joints; `*Convention` type names become `*Joint`; ordering attributes map to Dovetail's (for example `DependsOnJoint<T>`); `[LiveConvention]` and `[UnitTestConvention]` map to the Dovetail host-type attributes (see D5).
- Remove `Rocket.Surgery.Conventions*`, `Rocket.Surgery.Hosting`, `Rocket.Surgery.WebAssembly.Hosting` and `Rocket.Surgery.Extensions` references.
- Samples call the generated `ConfigureDovetail()`.
- Update docs and XML comments to the Joint vocabulary.

#### L6. Remove `RegisterOptionsConfigurationAttribute`

Labels: `:fire: enhancement`

- Delete `src/Primitives/RegisterOptionsConfigurationAttribute.cs` and `src/Primitives/Conventions/OptionsConvention.cs`. The attribute is also read by a reflection path through the Specular type provider.
- Port `test/Extensions.Tests/AutoRegisterOptions.cs` to `[DovetailConfiguration]`.
- Keep `ExistingValueOptions` / `ExistingValueOptionsFactory<T>`: used by the System.Text.Json and Newtonsoft conventions, unrelated.

#### L7. Delete `LaunchPad.Metadata`

Labels: `:fire: enhancement`

Remove `src/Metadata` (`AssemblyMetadataProvider`, `GitVersion`) and `test/Metadata.Tests`. No other project uses it. Three of its eleven tests are skipped. Typed access now comes from `AssemblyInfo.Metadata` and `AssemblyInfo.Dependencies` (M1).

#### L8. Rebuild `LaunchPad.Hosting` on host-model templates

Labels: `:rocket: feature`

Follow `Joinery.Dovetail.Hosting`: joints shipped as templates, emitted only when the host model and referenced libraries match, and only into projects with a **direct reference** to `LaunchPad.Hosting`.

- Initial host models: generic host, ASP.NET Core, Blazor WebAssembly. MAUI and Uno are out of scope until a sample needs them.
- Fold in `LaunchPad.WebAssembly.Hosting`: its Browser console sink, `Environment` and `BaseAddress` enrichment, and the `BannedSymbols` config that forbids `Microsoft.Extensions.Hosting` (today gated by `RocketSurgeryLaunchPadWebAssemblyHostingBanHosting`).
- Joint disposition is in the table below.
- Add template tests and snapshots modelled on Dovetail's `HostingHelperTests`; include a "transitive reference gets nothing" test.

#### L9. Replace `LaunchPad.Telemetry` with `Joinery.Dovetail.OpenTelemetry`

Labels: `:boom: breaking change`

- Delete `src/Telemetry` and its six interfaces and delegates, and the ten `ConfigureOpenTelemetry*` overloads.
- Add `InstrumentationJoint` to `LaunchPad.Hosting`: `HttpClient` tracing with `RecordException`, and `HttpClient`, runtime and process metrics. Runtime and process instrumentation are not emitted for Blazor WebAssembly.
- Port the AspNetCore and HotChocolate instrumentation joints to Dovetail's joint interfaces, emitted only when OpenTelemetry is referenced.
- Remove the `Primitives` to `Telemetry` project reference.

#### L10. Replace `LaunchPad.Serilog` with `Joinery.Dovetail.Serilog`

Labels: `:boom: breaking change`

- Delete `src/Serilog`. Port the opinionated joints into `LaunchPad.Hosting`, emitted when Serilog is referenced: enrichers (environment, span, exceptions, demystifier, log context), `ApplicationState:LogLevel` mapping, console/debug/trace sinks, `IDiagnosticContext`.
- Port `SerilogTelemetryConvention` (`WriteTo.OpenTelemetry`) as a live joint; it has never run, so this starts log export.
- Serilog-specific code in other packages moves out: AspNetCore's `SerilogLoggingActionFilter`, `SerilogLoggingPageFilter` and request logging; the NodaTime, Newtonsoft.Json, System.Text.Json and NetTopologySuite destructuring policies.
- Design points to settle in this issue: where each destructuring policy and the Serilog sink packages live (a `*.Serilog` integration package versus a Serilog-gated template, and whether the SDK or `LaunchPad.Hosting` brings the sinks).
- Remove the `Primitives` to `Serilog` project reference, which forces Serilog on every consumer today.

#### L11. Split NodaTime into integration packages

Labels: `:boom: breaking change`

| Package                              | Contents (from today)                                                                                                   | Added when also referenced |
| ------------------------------------ | ----------------------------------------------------------------------------------------------------------------------- | -------------------------- |
| `LaunchPad.NodaTime`                 | `TimeConvention` clock and time-zone registration, STJ converters (`Primitives`), `PrimitiveOptions.DateTimeZoneSource` | NodaTime                   |
| `LaunchPad.NodaTime.NewtonsoftJson`  | converters and extensions from `Foundation.NewtonsoftJson`                                                              | Newtonsoft.Json            |
| `LaunchPad.NodaTime.Mapping`         | `NodaTimeMapper`, `NodaTimeDateTimeMapper`                                                                              | Riok.Mapperly              |
| `LaunchPad.NodaTime.EntityFramework` | `NodaTimeDateTimeOffsetValueGenerator`, `NodaTimeDateTimeValueGenerator`                                                | EF Core                    |
| `LaunchPad.NodaTime.HotChocolate`    | `AddNodaTime`, the time-zone cache setup                                                                                | HotChocolate               |
| `LaunchPad.NodaTime.StrawberryShake` | 14 serializers (`DateTimeZone`, `Duration`, `Instant`, `IsoDayOfWeek`, `LocalDate`, ... `ZonedDateTime`, `Period`)      | StrawberryShake            |
| `LaunchPad.NodaTime.Serilog`         | `NodaTimeDestructuringPolicy`                                                                                           | Serilog                    |

Also: `DateTimeZoneSerializer` and `OffsetDateSerializer` exist but are not registered by `StrawberryShakeConvention`; decide whether to register them. Move the matching tests out of `Extensions.Tests`, `Sample.Graphql.Tests/StrawberryShakeSerializerTests` and the mapping tests. `Verify.NodaTime` stays a test dependency.

#### L12. Extract FluentValidation into integration packages

Labels: `:boom: breaking change`

FluentValidation is embedded in `Foundation`, `AspNetCore`, `Grpc` and `HotChocolate` today, so the SDK cannot add "FluentValidation packages" until it is extracted. MediatR itself stays in `Foundation`.

| Package                                   | Contents                                                                                                                                     | Added when also referenced |
| ----------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------- |
| `LaunchPad.FluentValidation`              | `FluentValidationConvention`, `Validation/*` (pipeline behavior, `CompositeValidator`, `StringInValidator`), `FluentValidationProblemDetail` | FluentValidation           |
| `LaunchPad.FluentValidation.AspNetCore`   | `AspNetCore` validation code and `AspNetCoreValidationBehaviorConvention`; absorbs `AspNetCore.FluentValidation.OpenApi` (experimental)      | ASP.NET Core               |
| `LaunchPad.FluentValidation.Grpc`         | `Grpc` `Validation/*`                                                                                                                        | Grpc.AspNetCore            |
| `LaunchPad.FluentValidation.HotChocolate` | FairyBread integration from `GraphqlConvention`                                                                                              | HotChocolate               |
| `LaunchPad.FluentValidation.Blazor`       | the `FluentValidator` component (all that `AspNetCore.Blazor` contains)                                                                      | Blazor components          |

Remove the hard `AspNetCore` to `AspNetCore.FluentValidation.OpenApi` project reference.

#### L13. Tidy Newtonsoft.Json and NetTopologySuite packages

Labels: `:fire: enhancement`

- Keep `Foundation.NewtonsoftJson`, `AspNetCore.NewtonsoftJson`, `Mapping.NewtonsoftJson`, `Spatial`, `Spatial.NewtonsoftJson` and `StrawberryShake.Spatial`.
- Delete `AspNetCore.Spatial`: its convention body and all of `NetTopologySuiteSwashbuckleExtensions.cs` are commented out.
- Move NTS and Newtonsoft Serilog code out (see L10); fix the misspelled `SpactialNewtonsoftJsonConvention.cs`.
- Remove the unused `FoundationOptions` field from `AspNetCore.NewtonsoftJson`.

#### L14. Update the samples

Labels: `:hammer: chore`

- Every sample calls `ConfigureDovetail()` (L5).
- `Sample.Pages` is restored (done on this branch) and works again with `Sample.Pages.Tests`.
- Rebuild `Sample.Command` on a plain generic host with Spectre.Console.Cli and `Joinery.Dovetail.DryIoc`; drop `Rocket.Surgery.CommandLine` and `Conventions.DryIoc`.
- Sample.BlazorWasm moves to the folded WebAssembly templates.

### Phase 4: LaunchPad SDK

#### L15. Create `Rocket.Surgery.LaunchPad.Sdk`

Labels: `:rocket: feature`

Model it on `Joinery.Dovetail.Sdk` and `Rocket.Surgery.Sdk`.

- Applied from `Directory.Build.props` with a `global.json` `msbuild-sdks` entry; imports `Joinery.Dovetail.Sdk` so apps need one SDK entry.
- Reuses Dovetail's host detection to add `LaunchPad.Hosting` for any detected host. Defaults `DovetailEnableSerilog` to true when Serilog is referenced directly.
- Adds integration packages from **direct** `PackageReference`s only, expanded as implicit references at evaluation time. Default on, with `LaunchPadEnable<Tech>=false` to opt out.
- Package versions come from a generated `Packages.props` at pack time, as in Dovetail.
- Tests with `MSBuild.ProjectCreation` and Verify snapshots; a sample that consumes the packed SDK like an outside user (compare `sample/SdkSample`).

Detection rules (all packages are `Rocket.Surgery.LaunchPad.<name>`):

| Technology detected | Packages added (each also needs its partner library referenced, where shown)                                                                                    |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| NodaTime            | `NodaTime`; `.NewtonsoftJson` (Newtonsoft.Json); `.Mapping` (Mapperly); `.EntityFramework` (EF Core); `.HotChocolate`; `.StrawberryShake`; `.Serilog` (Serilog) |
| Newtonsoft.Json     | `Foundation.NewtonsoftJson`; `AspNetCore.NewtonsoftJson` (ASP.NET Core); `Mapping.NewtonsoftJson` (Mapperly); `Spatial.NewtonsoftJson` (NetTopologySuite)       |
| Serilog             | Dovetail's Serilog package (via the chained SDK); `NodaTime.Serilog` (NodaTime); the Serilog destructuring packages from L10                                    |
| NetTopologySuite    | `Spatial`; `Spatial.NewtonsoftJson` (Newtonsoft.Json); `StrawberryShake.Spatial` (StrawberryShake); the NTS Serilog package (Serilog)                           |
| FluentValidation    | `FluentValidation`; `.AspNetCore`; `.Grpc`; `.HotChocolate`; `.Blazor` (each when its host library is referenced)                                               |

Other libraries (HotChocolate, Grpc, EF Core, Mapperly, MediatR) are referenced explicitly in v1.

### Phase 5: docs, tooling, release

#### L16. Docs content and API reference

Labels: `:blue_book: documentation`

The empty Starlight shell is in place (Getting Started, Concepts, Changelog; no theme, no API reference).

- Write guides and concepts in the Joint vocabulary; add pages for the SDK, host models and each integration.
- Add the C# API reference: a `docs:api` task using `Rocket.Surgery.ApiReference`, the `packages` collection and `src/pages/api` routes, as in Dovetail and Specular.
- Add a brand theme and wordmarks.

#### L17. Agent and editor tooling follow-ups

Labels: `:hammer: chore`

- Sync the `apm.yml` package list and `Skillfile` with Dovetail and run `apm install`.
- Add `AGENTS.md` and `CLAUDE.md`.
- Replace the prettier extension recommendation with biome (`.vscode/extensions.json`, `.devcontainer/devcontainer.json`).
- Decide whether to port Dovetail's `samples.yml`.
- Normalise `NuGet.config` (BOM) through the hk step.

#### L18. Release: major version and NuGet deprecations

Labels: `:boom: breaking change`

- Choose the new major version in `GitVersion.yml`.
- Deprecate on nuget.org: Telemetry, Metadata, Serilog, Testing, AspNetCore.Testing, WebAssembly.Hosting, AspNetCore.Spatial and AspNetCore.FluentValidation.OpenApi, pointing at their replacements.
- Write migration notes (Convention to Joint, package map).

#### L19. Repository settings checklist (manual)

Labels: `:hammer: chore`

I cannot change these; the new `build.yml` needs them:

- GitHub Pages serving the `gh-pages` branch.
- A `nuget` environment, a `NUGET_USER` secret, and a nuget.org trusted-publishing policy naming `build.yml` and that environment.
- Labels (`sync-labels.yml`), Codecov token, and merge-queue settings.

---

## Hosting joint disposition (L8)

| Today                                                 | Destination                                                                                                                |
| ----------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------- |
| `HostingConvention` (registers the lifecycle service) | Dovetail's lifecycle support (D1); nothing LaunchPad-side                                                                  |
| `SerilogHostingConvention` (`IDiagnosticContext`)     | Hosting joint, emitted when Serilog is referenced                                                                          |
| `HealthChecksConvention`                              | Hosting joint for hosts with `Microsoft.Extensions.Diagnostics.HealthChecks` (generic host, ASP.NET Core); not WebAssembly |
| `DefaultTelemetryConvention`                          | Dropped if D3 confirms the SDK defaults cover it                                                                           |
| `EnvironmentLoggingConvention`                        | Hosting joint when Serilog is referenced; WebAssembly variant enriches `Environment` and `BaseAddress`                     |
| `SerilogConsole/Debug/TraceLoggingConvention`         | Hosting joints when Serilog is referenced; WebAssembly uses the Browser console sink                                       |
| `SerilogTelemetryConvention` (never exported)         | Live joint, emitted when `Serilog.Sinks.OpenTelemetry` is referenced                                                       |
| `InstrumentationConvention` (from Telemetry)          | `InstrumentationJoint` (L9)                                                                                                |

## Current package disposition

| Project                             | Fate                                                                                   |
| ----------------------------------- | -------------------------------------------------------------------------------------- |
| Analyzers                           | Keep (`netstandard2.0`; six generators)                                                |
| Primitives                          | Keep, slimmed: no Serilog/Telemetry references, NodaTime and options attribute removed |
| Foundation                          | Keep (MediatR, core); FluentValidation extracted                                       |
| Foundation.NewtonsoftJson           | Keep; NodaTime and Serilog parts move out                                              |
| Metadata                            | Delete (L7)                                                                            |
| Hosting                             | Rebuilt on templates (L8); absorbs WebAssembly.Hosting                                 |
| WebAssembly.Hosting                 | Folded into Hosting                                                                    |
| Telemetry                           | Delete (L9)                                                                            |
| Serilog                             | Delete (L10)                                                                           |
| AspNetCore                          | Keep; FluentValidation and Serilog parts move out                                      |
| AspNetCore.Blazor                   | Becomes `FluentValidation.Blazor`                                                      |
| AspNetCore.FluentValidation.OpenApi | Merged into `FluentValidation.AspNetCore`                                              |
| AspNetCore.NewtonsoftJson           | Keep                                                                                   |
| AspNetCore.Spatial                  | Delete (empty shell)                                                                   |
| AspNetCore.Testing, Testing         | Delete; test support under `test/` (L4)                                                |
| EntityFramework                     | Keep; NodaTime value generators move to `NodaTime.EntityFramework`                     |
| Grpc                                | Keep; validation moves to `FluentValidation.Grpc`                                      |
| HotChocolate                        | Keep; NodaTime and FluentValidation parts move out                                     |
| Mapping, Mapping.NewtonsoftJson     | Keep; NodaTime mappers move out                                                        |
| Spatial, Spatial.NewtonsoftJson     | Keep; Serilog parts move out                                                           |
| StrawberryShake                     | NodaTime serializers move to `NodaTime.StrawberryShake`; generic serializers stay      |
| StrawberryShake.Spatial             | Keep                                                                                   |

## Open design points

1. **Specular access from a joint**: how a joint obtains its `ISpecularProvider` (L2, L5).
2. **Phase 1 coexistence**: the green baseline may keep `Rocket.Surgery.Conventions` for wiring while scanning moves to Specular. If the two cannot coexist, fold the affected projects' Phase 1 work into L5.
3. **Template gating**: reuse a Dovetail extension point or reimplement (D4).
4. **Serilog placement**: sink packages and destructuring policies (L10).
5. **AssemblyInfo representation**: symbols versus embedded data (M1).

## Risks

- **net11 prerelease support**: HotChocolate, StrawberryShake, Alba and EF Core may lack net11 assets; mitigated by TFM-conditional versions and a per-dependency list in L1.
- **Moving upstream**: Dovetail, Specular, Testing and MSBuildSdk are mid net11 migration. LaunchPad's branch is also based on a commit 214 behind `origin/main`; rebase after the baseline.
- **Test conversion volume**: 309 tests, some using `Xunit.Combinatorial`.
- **Breaking release**: nearly every package changes; L18 owns the migration notes.

## Out of scope

- SDK auto-detection beyond the five technologies (HotChocolate, Grpc, EF Core, Mapperly, MediatR).
- MAUI and Uno host models.
- Redesign of testing support (you plan this separately).
