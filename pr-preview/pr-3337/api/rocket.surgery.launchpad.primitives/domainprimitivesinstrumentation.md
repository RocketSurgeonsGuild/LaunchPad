# DomainPrimitivesInstrumentation

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1324`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Diagnostics`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.09dd538c83fe5dc2188f0956cd1425be399f85c1/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs)

The metrics and trace events of domain primitives, which the generated `Create` and `TryCreate` record. Nothing is recorded until a listener subscribes: `AddLaunchPadPrimitivesInstrumentation()` on OpenTelemetry's meter and tracer provider builders, which the Primitives generator writes when the application references OpenTelemetry, or a `Metrics.MeterListener` / `Diagnostics.ActivityListener` of [DomainPrimitivesInstrumentation.Name](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Diagnostics;

public static class DomainPrimitivesInstrumentation
{
    // ...
}
```

## Properties

- [ActivitySource](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/properties.md#activitysource) : `ActivitySource` `static` `get` -- The activity source. A rejected value adds a `domain_primitive.validation.failure` event to the current activity while it has a listener.
- [Meter](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/properties.md#meter) : `Meter` `static` `get` -- The meter.

## Methods

- [RecordCreation(string)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/methods.md#recordcreation-string) `static` -- Records that a domain primitive was created.
- [RecordValidationFailure(string, PrimitiveError)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/methods.md#recordvalidationfailure-string-primitiveerror) `static` -- Records that a domain primitive rejected a value.
- [RecordValidationSuccess(string)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/methods.md#recordvalidationsuccess-string) `static` -- Records that a domain primitive accepted a value.

## Fields

- [Name](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name) : `string` `static` -- The name of the meter and the activity source.
