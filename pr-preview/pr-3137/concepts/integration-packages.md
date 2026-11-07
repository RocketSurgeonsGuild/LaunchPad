# Integration packages

An **integration package** adds LaunchPad support for one third-party technology on one surface. NodaTime has one package for
the clock and serialization, `Rocket.Surgery.LaunchPad.NodaTime`, and one for each library it works with:
`Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`, `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`,
`Rocket.Surgery.LaunchPad.NodaTime.EntityFramework` and so on. One technology usually has several.

Each package exports the [Joints](../joints/) for its pair of technologies and depends on both. Nothing in it is optional, so
a package is tested, versioned and found like any other. The alternative, one package per technology that ships its integrations
as templates, was rejected for that reason (see
[ADR 0002](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0002-package-per-integration.md)). The cost is many
small packages, which the [LaunchPad SDK](../launchpad-sdk/) chooses for you.

## The packages the SDK chooses

| The project references          | The SDK adds                                                                                 |
| ------------------------------- | -------------------------------------------------------------------------------------------- |
| a [host model](../host-models/) | `Rocket.Surgery.LaunchPad.Hosting`                                                           |
| the `WebApplication` host model | `Rocket.Surgery.LaunchPad.AspNetCore`                                                        |
| `NodaTime`                      | `.NodaTime`, and `.NodaTime.HotChocolate` / `.NodaTime.StrawberryShake` with those libraries |
| `NetTopologySuite`              | `.Spatial`, and `.StrawberryShake.Spatial` with StrawberryShake                              |
| `FluentValidation`              | `.FluentValidation`, and its ASP.NET Core, gRPC, HotChocolate and Blazor packages with those |
| `HotChocolate`                  | `.HotChocolate`                                                                              |
| `StrawberryShake`               | `.StrawberryShake`                                                                           |
| `Grpc.AspNetCore`               | `.Grpc`                                                                                      |

A file-based app (`dotnet run app.cs`) also gets `Rocket.Surgery.LaunchPad.Foundation` and counts as referencing NodaTime.

Each technology can be turned off with its `LaunchPadEnable<Technology>` property, which removes its own package and every
integration package it is part of:

```xml
<PropertyGroup>
  <LaunchPadEnableNodaTime>false</LaunchPadEnableNodaTime>
</PropertyGroup>
```

The properties are `LaunchPadEnableHosting`, `LaunchPadEnableFoundation`, `LaunchPadEnableAspNetCore`, `LaunchPadEnableNodaTime`,
`LaunchPadEnableNetTopologySuite`, `LaunchPadEnableFluentValidation`, `LaunchPadEnableGrpc`, `LaunchPadEnableHotChocolate` and
`LaunchPadEnableStrawberryShake`.

## Direct references

A **direct reference** is a package or project reference a project declares itself, as opposed to one it only receives through
another package or project. The SDK and the [templates](../templates/) act on direct references only: a library the project
receives transitively was chosen by whoever referenced it, not by the project, so it adds nothing to the project.

If a technology arrives only transitively and you want its LaunchPad support, reference it directly.