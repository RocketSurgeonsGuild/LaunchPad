# DomainPrimitivesInstrumentation Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitivesInstrumentation](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation.md)
- Kind: `Properties`
- Members: `2`

The metrics and trace events of domain primitives, which the generated `Create` and `TryCreate` record. Nothing is recorded until a listener subscribes: `AddLaunchPadPrimitivesInstrumentation()` on OpenTelemetry's meter and tracer provider builders, which the Primitives generator writes when the application references OpenTelemetry, or a `Metrics.MeterListener` / `Diagnostics.ActivityListener` of [DomainPrimitivesInstrumentation.Name](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name).

<a id="activitysource"></a>

## ActivitySource

- Name: `ActivitySource`
- Modifiers: `static` `get`
- Returns: `ActivitySource`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.7307fcdde362b2224f6c0071b6dfb1eaf21e5cff/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs#L29)

The activity source. A rejected value adds a `domain_primitive.validation.failure` event to the current activity while it has a listener.

```csharp
public static ActivitySource ActivitySource { get; }
```

<a id="meter"></a>

## Meter

- Name: `Meter`
- Modifiers: `static` `get`
- Returns: `Meter`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.7307fcdde362b2224f6c0071b6dfb1eaf21e5cff/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs#L23)

The meter.

```csharp
public static Meter Meter { get; }
```
