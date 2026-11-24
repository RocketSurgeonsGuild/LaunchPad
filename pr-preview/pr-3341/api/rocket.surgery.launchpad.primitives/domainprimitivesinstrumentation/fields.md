# DomainPrimitivesInstrumentation Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitivesInstrumentation](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation.md)
- Kind: `Fields`
- Members: `1`

The metrics and trace events of domain primitives, which the generated `Create` and `TryCreate` record. Nothing is recorded until a listener subscribes: `AddLaunchPadPrimitivesInstrumentation()` on OpenTelemetry's meter and tracer provider builders, which the Primitives generator writes when the application references OpenTelemetry, or a `Metrics.MeterListener` / `Diagnostics.ActivityListener` of [DomainPrimitivesInstrumentation.Name](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name).

<a id="name"></a>

## Name

- Name: `Name`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs)

The name of the meter and the activity source.

```csharp
public const static string Name
```
