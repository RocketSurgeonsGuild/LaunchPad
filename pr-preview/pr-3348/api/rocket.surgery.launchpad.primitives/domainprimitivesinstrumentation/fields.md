# DomainPrimitivesInstrumentation Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitivesInstrumentation](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation.md)
- Kind: `Fields`
- Members: `1`

The metrics and trace events of domain primitives, which the generated `Create` and `TryCreate` record. Nothing is recorded until a listener subscribes: `AddLaunchPadPrimitivesInstrumentation()` on OpenTelemetry's meter and tracer provider builders, which the Primitives generator writes when the application references OpenTelemetry, or a `Metrics.MeterListener` / `Diagnostics.ActivityListener` of [DomainPrimitivesInstrumentation.Name](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name).

<a id="name"></a>

## Name

- Name: `Name`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3348-merge.Sha.90dd4871ffef6c7ec9cccc3065d35dfa4da5b954/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs)

The name of the meter and the activity source.

```csharp
public const static string Name
```
