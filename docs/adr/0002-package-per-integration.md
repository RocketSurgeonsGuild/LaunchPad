# One package per third-party integration, not one package with gated templates

Optional support for a third-party technology (NodaTime, Newtonsoft.Json, NetTopologySuite, FluentValidation, Serilog) ships as one package per technology and surface, for example `Rocket.Surgery.LaunchPad.NodaTime.EntityFramework`. The LaunchPad SDK adds the packages whose libraries a project references directly.

The alternative was one package per technology that ships its integration code as templates, emitted only when the partner library is present. We chose packages because they can be tested, versioned and discovered like any other package, and they match the existing `*.NewtonsoftJson` and `Spatial.*` structure. Templates stay reserved for host-model gating inside `LaunchPad.Hosting`, where no package boundary exists.

The consequence is many small packages; the SDK hides that count from applications.
