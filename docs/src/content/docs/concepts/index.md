---
title: Concepts
description: The ideas behind LaunchPad and how it builds on Specular and Dovetail.
---

LaunchPad assembles two libraries into ready-to-use wiring for .NET applications:

- [Dovetail](https://rocketsurgeonsguild.github.io/Dovetail/) composes an application at build time from **Joints**, and its SDK
  works out what kind of application a project is.
- [Specular](https://rocketsurgeonsguild.github.io/Specular/) scans assemblies without reflection, so the wiring stays trimming and
  Native AOT safe.

LaunchPad adds the Joints most applications want (health checks, logging, telemetry, validation, time, serialization) and an SDK
that picks the right packages for a project. These pages explain the vocabulary the rest of the documentation uses.

| Term                                                          | In one line                                                                                    |
| ------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| [Joint](./joints/)                                            | A unit of application wiring that Dovetail resolves and applies at build time.                 |
| [Host model](./host-models/)                                  | The kind of application a project is, decided at build time. It determines which Joints apply. |
| [Integration package](./integration-packages/)                | A package that adds support for one third-party technology on one surface.                     |
| [Direct reference](./integration-packages/#direct-references) | A reference the project declares itself, as opposed to one it receives transitively.           |
| [Template](./templates/)                                      | Source written into a project only when its host model and references match.                   |
| [LaunchPad SDK](./launchpad-sdk/)                             | The MSBuild SDK that chooses LaunchPad packages from what a project references.                |
