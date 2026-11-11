# One package per third-party integration, not one package with gated templates

Optional support for a third-party technology (NodaTime, Newtonsoft.Json, NetTopologySuite, FluentValidation, Serilog) ships as one package per technology and surface, for example `Rocket.Surgery.LaunchPad.NodaTime.EntityFramework`. The LaunchPad SDK adds the packages whose libraries a project references directly.

The alternative was one package per technology that ships its integration code as templates, emitted only when the partner library is present. We chose packages because they can be tested, versioned and discovered like any other package, and they match the existing `*.NewtonsoftJson` and `Spatial.*` structure. Templates stay reserved for host-model gating inside `LaunchPad.Hosting`, where no package boundary exists.

The consequence is many small packages; the SDK hides that count from applications.

## Amendment (2026-11-08): the API generators are one package

The mediator-agnostic API generators ship as one package, `Rocket.Surgery.LaunchPad.Api`, rather than one package per transport. They aren't integrations with runtime code: they are compile-time generators that share most of their code (inference, naming, mediator profiles, binding, diagnostics, emit helpers). As separate packages, that core was compiled into every analyzer assembly as linked source, which the formatter and the IDE handled badly and which every change had to keep in step. In one assembly the core is ordinary source. Each generator still only writes code for hosts of its own transport, using that transport's library, so referencing the package costs a project nothing it doesn't use. The LPAD1010 code fix is in the same assembly, against RS1038's advice: the compiler only reads the assembly's generators and analyzers from metadata and never creates the fix, so it loads without the Workspaces assembly the fix needs (design doc, item 14). Runtime integrations keep one package per technology.

## Amendment (2026-11-10): Primitives ships its Joints as templates

`Rocket.Surgery.LaunchPad.Primitives`, the strong types package, ships the Joints that describe strong types to OpenAPI, HotChocolate and EF Core as templates, not as integration packages, so it depends on none of those libraries ([ADR 0008](0008-primitives-is-an-independent-strong-types-package.md)).
