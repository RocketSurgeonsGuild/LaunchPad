# AGENTS.md

Guidance for AI coding agents (Claude Code, GitHub Copilot and others) working in this repository. Read the code and the tickets for the current state; this file records only what is stable enough to write down.

## What this repository is

LaunchPad is a set of `Rocket.Surgery.LaunchPad.*` libraries for building .NET console and web applications, published by the Rocket Surgeons Guild.

It is being rebuilt on two building blocks, tracked by the epic [RocketSurgeonsGuild/LaunchPad#3045](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3045) and the plan in [`plans/dovetail-migration.md`](plans/dovetail-migration.md):

- **Specular** provides AOT-safe assembly scanning.
- **Dovetail** provides build-time composition. A **Joint** is a unit of application wiring that Dovetail resolves and applies at build time; it replaces what LaunchPad calls a convention today.

This is a migration in progress, so both models exist at once. The pre-migration code is convention-based: `Rocket.Surgery.Conventions` is a central package version in `Directory.Packages.props`, and the convention classes live in `Conventions/` folders inside the packages (for example `src/Hosting/Conventions`). Which packages have moved is tracked by the epic's sub-issues.

- Use the vocabulary in [`GLOSSARY.md`](GLOSSARY.md) (Joint, host model, integration package, direct reference, template, LaunchPad SDK) in code and docs you touch, and avoid the terms it marks as avoided.
- The reasons behind the two big decisions are in [`docs/adr/`](docs/adr): Joints replace conventions, and one package per integration.
- Package IDs and namespaces stay `Rocket.Surgery.LaunchPad.*`.
- The convention-to-Joint conversion is sequenced in batches by the epic. Convert a package only under the ticket that owns it.
- Every test project runs on TUnit with Microsoft.Testing.Platform, as moved by tickets [#3050](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3050) and [#3051](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3051); `test/Directory.Build.targets` adds the TUnit, Verify.TUnit, FakeItEasy and Shouldly packages to each of them, and there is no xunit anywhere. The same file chooses the wiring generator per test project: a project that sets `UseDovetail` in its project file gets `Joinery.Dovetail` and Specular instead of `Rocket.Surgery.Conventions` (the test project of each converted package does; each conversion batch flips its own test projects and [#3056](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3056) deletes the property). Shared test code (the web app fixtures and fake time) lives in the non-packable `test/TestSupport` project. A test creates scopes directly with `CreateScope()` / `CreateAsyncScope()`. Check what a test project references before assuming its wiring.

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

The solution is `LaunchPad.slnx`. Whether it restores and builds cleanly is the baseline work in [#3046](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3046); check its state before assuming a green build.

## Build, test and format

- Tools come from mise: `mise install` installs what `.config/mise.toml` pins (the .NET SDK that `global.json` asks for, node, hk, biome, oxfmt and so on). `global.json` also selects the Microsoft.Testing.Platform test runner.
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
