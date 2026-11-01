# AGENTS.md

Guidance for AI coding agents (Claude Code, GitHub Copilot and others) working in this repository. Read the code and the tickets for the current state; this file records only what is stable enough to write down.

## What this repository is

LaunchPad is a set of `Rocket.Surgery.LaunchPad.*` libraries for building .NET console and web applications, published by the Rocket Surgeons Guild.

It is being rebuilt on two building blocks, tracked by the epic [RocketSurgeonsGuild/LaunchPad#3045](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3045) and the plan in [`plans/dovetail-migration.md`](plans/dovetail-migration.md):

- **Specular** provides AOT-safe assembly scanning.
- **Dovetail** provides build-time composition. A **Joint** is a unit of application wiring that Dovetail resolves and applies at build time; it replaces what LaunchPad used to call a convention.

The conversion is done: no project references `Rocket.Surgery.Conventions` or the old `Rocket.Surgery.Hosting` and `Rocket.Surgery.Extensions` packages any more, and on CI the whole solution builds and its tests pass on both frameworks ([#3056](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3056)). What is left of the epic (the typed assembly info that replaces `LaunchPad.Metadata`, the integration package splits in [`plans/dovetail-migration.md`](plans/dovetail-migration.md), the LaunchPad SDK and the docs) is tracked by its sub-issues. The build pipeline in `build/` is the one exception: `Rocket.Surgery.ModularPipelines.Extensions` is still convention-based, and it is tooling outside the solution.

- Use the vocabulary in [`GLOSSARY.md`](GLOSSARY.md) (Joint, host model, integration package, direct reference, template, LaunchPad SDK) in code and docs you touch, and avoid the terms it marks as avoided.
- The reasons behind the two big decisions are in [`docs/adr/`](docs/adr): Joints replace conventions, and one package per integration.
- Package IDs and namespaces stay `Rocket.Surgery.LaunchPad.*`.
- The remaining package moves are sequenced by the epic. Move a package only under the ticket that owns it.
- `LaunchPad.Hosting` ships its Joints as **templates** (`src/Hosting/build/*.cs.template`), not as compiled code, so the package depends on none of what they use. `src/Hosting/build/Rocket.Surgery.LaunchPad.Hosting.targets` declares them with Dovetail's `DovetailTemplate` item: a template is written into a project only if it has a direct reference to Hosting, declares one of the template's host models and references the assemblies it compiles against. NuGet imports those targets for a package reference; the root `Directory.Build.targets` imports them for the project references the samples and tests use. The header of the targets file says how to add a template, and `test/Hosting.Tests` packs the package and builds throwaway applications to prove each gate.
- `src/Sdk` is the **LaunchPad SDK** (`Rocket.Surgery.LaunchPad.Sdk`), an MSBuild SDK an application applies in its `Directory.Build.props`. It chains `Joinery.Dovetail.Sdk` (the version `Directory.Packages.props` pins for it) and adds LaunchPad packages from what Dovetail's host detection finds and from the project's direct `PackageReference`s; `LaunchPadEnable<Tech>=false` opts out. The versions are written into `Sdk/Packages.props` when the SDK is packed. `test/Sdk.Tests` packs it and builds throwaway applications against it, and `sample/Sample.Sdk` consumes it from packed packages (`dotnet msbuild build/pack.proj` there first). Like an application outside the repository, that sample and the throwaway projects of the tests stand outside central package management and name their own versions.
- Every test project runs on TUnit with Microsoft.Testing.Platform, as moved by tickets [#3050](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3050) and [#3051](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3051); `test/Directory.Build.targets` adds the TUnit, Verify.TUnit, FakeItEasy and Shouldly packages to each of them, and there is no xunit anywhere. The same file gives every test project `Joinery.Dovetail` and Specular, so a test gets the joints and the Specular scan results of the projects it references. Shared test code (the web app fixtures and fake time) lives in the non-packable `test/TestSupport` project. A test creates scopes directly with `CreateScope()` / `CreateAsyncScope()`.

## Repository layout

| Path                 | Contents                                                                                                                             |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| `src/`               | One folder per package (`Foundation`, `Hosting`, `AspNetCore`, `Grpc`, and so on)                                                    |
| `test/`              | Test projects, including `Sample.*.Tests` for the samples                                                                            |
| `sample/`            | `Sample.*` applications                                                                                                              |
| `build/`             | The build pipeline: `Build.cs` is a file-based app that runs ModularPipelines (`Samples.cs` has the AOT publish of selected samples) |
| `docs/`              | The Astro/Starlight documentation site, plus the ADRs in `docs/adr/`                                                                 |
| `.config/`           | `mise.toml` (tools and tasks) and `hk.pkl` (lint and format steps)                                                                   |
| `.github/workflows/` | CI: `build.yml` runs `mise run build`, `lint.yml` runs `hk` through autofix.ci                                                       |

The solution is `LaunchPad.slnx`, and it builds cleanly on CI (linux x64). On macOS arm64 without Rosetta the gRPC sample does not build, because `Grpc.Tools` ships only an x64 `protoc` for macOS.

## Build, test and format

- Tools come from mise. `mise install` installs what `.config/mise.toml` pins: the .NET SDK that `global.json` asks for, node, hk, biome, oxfmt and so on. `global.json` also selects the Microsoft.Testing.Platform test runner.
- mise sets `DOTNET_ROLL_FORWARD=Major`. StrawberryShake's code generator targets .NET 9, and this lets it run on the .NET 10 runtime.
- Build with `mise run build`, which runs `dotnet run build/Build.cs`. The docs site has its own tasks: `mise run docs`, `mise run docs:build` and `mise run docs:preview`.
- Package versions are central: add them to `Directory.Packages.props` (samples have their own `sample/Directory.Packages.props`) and never put a `Version` in a `.csproj`.
- Formatting and linting run through hk (`.config/hk.pkl`): `hk check` verifies and `hk fix` applies fixes. Biome formats JavaScript, TypeScript and JSON, oxfmt formats YAML, Markdown, CSS and HTML (style and ignores in `.oxfmtrc.json`), tombi formats TOML and `dotnet format` handles C#. Generated agent files are excluded.
- `.editorconfig` holds the editor settings. The recommended editor extension for formatting is Biome (`biomejs.biome`).

## Agent tooling

The agent packages and skills are managed the same way as in [Dovetail](https://github.com/RocketSurgeonsGuild/Dovetail):

- `apm.yml` lists the agent packages and `Skillfile` declares the install targets (Claude Code and Copilot, locally).
- `.config/mise.toml` pins the `apm` and `skillfile` tools.
- Install with `mise install`, then `mise exec -- apm install` and `mise exec -- skillfile install`.
- `apm.lock.yaml` and the files apm deploys (skills and agents under `.agents/`, `.claude/` and `.github/`) are committed. Regenerate them with the commands above instead of editing them by hand.
- `apm_modules/` and the machine-local files `apm install` writes (listed in `.gitignore`) are not committed.

Update this file in the same pull request as any change that makes a statement here untrue.
