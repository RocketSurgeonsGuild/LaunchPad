# DomainPrimitivesInstrumentation Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitivesInstrumentation](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation.md)
- Kind: `Methods`
- Members: `3`

The metrics and trace events of domain primitives, which the generated `Create` and `TryCreate` record. Nothing is recorded until a listener subscribes: `AddLaunchPadPrimitivesInstrumentation()` on OpenTelemetry's meter and tracer provider builders, which the Primitives generator writes when the application references OpenTelemetry, or a `Metrics.MeterListener` / `Diagnostics.ActivityListener` of [DomainPrimitivesInstrumentation.Name](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name).

<a id="recordcreation"></a>
<a id="recordcreation-string"></a>

## RecordCreation(string)

- Name: `RecordCreation(string)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs#L37-L39)

Records that a domain primitive was created.

```csharp
public static class DomainPrimitivesInstrumentation
{
    public static void RecordCreation(
        string primitiveName)
    {
        // ...
    }
}
```

## Parameters

- `primitiveName` (`string`)
  The domain primitive.

<a id="recordvalidationfailure"></a>
<a id="recordvalidationfailure-string-primitiveerror"></a>

## RecordValidationFailure(string, PrimitiveError)

- Name: `RecordValidationFailure(string, PrimitiveError)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs#L58-L79)

Records that a domain primitive rejected a value.

```csharp
public static class DomainPrimitivesInstrumentation
{
    public static void RecordValidationFailure(
        string primitiveName,
        PrimitiveError error)
    {
        // ...
    }
}
```

## Parameters

- `primitiveName` (`string`)
  The domain primitive.
- `error` ([PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  Why.

<a id="recordvalidationsuccess"></a>
<a id="recordvalidationsuccess-string"></a>

## RecordValidationSuccess(string)

- Name: `RecordValidationSuccess(string)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Primitives/Diagnostics/DomainPrimitivesInstrumentation.cs#L47-L49)

Records that a domain primitive accepted a value.

```csharp
public static class DomainPrimitivesInstrumentation
{
    public static void RecordValidationSuccess(
        string primitiveName)
    {
        // ...
    }
}
```

## Parameters

- `primitiveName` (`string`)
  The domain primitive.
