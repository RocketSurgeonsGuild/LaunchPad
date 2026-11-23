using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Rocket.Surgery.LaunchPad.Primitives.Diagnostics;

/// <summary>
///     The metrics and trace events of domain primitives, which the generated <c>Create</c> and <c>TryCreate</c> record. Nothing is recorded
///     until a listener subscribes: <c>AddLaunchPadPrimitivesInstrumentation()</c> on OpenTelemetry's meter and tracer provider builders, which
///     the Primitives generator writes when the application references OpenTelemetry, or a <see cref="MeterListener" /> /
///     <see cref="ActivityListener" /> of <see cref="Name" />.
/// </summary>
[PublicAPI]
public static class DomainPrimitivesInstrumentation
{
    /// <summary>
    ///     The name of the meter and the activity source.
    /// </summary>
    public const string Name = "Rocket.Surgery.LaunchPad.Primitives";

    /// <summary>
    ///     The meter.
    /// </summary>
    public static Meter Meter { get; } = new(Name, typeof(DomainPrimitivesInstrumentation).Assembly.GetName().Version?.ToString());

    /// <summary>
    ///     The activity source. A rejected value adds a <c>domain_primitive.validation.failure</c> event to the current activity while it has
    ///     a listener.
    /// </summary>
    public static ActivitySource ActivitySource { get; } = new(Name, typeof(DomainPrimitivesInstrumentation).Assembly.GetName().Version?.ToString());

    /// <summary>
    ///     Records that a domain primitive was created.
    /// </summary>
    /// <param name="primitiveName">The domain primitive.</param>
    public static void RecordCreation(string primitiveName)
    {
        if (!Creation.Enabled) return;
        Creation.Add(1, new KeyValuePair<string, object?>(PrimitiveTypeTag, primitiveName));
    }

    /// <summary>
    ///     Records that a domain primitive accepted a value.
    /// </summary>
    /// <param name="primitiveName">The domain primitive.</param>
    public static void RecordValidationSuccess(string primitiveName)
    {
        if (!ValidationSuccess.Enabled) return;
        ValidationSuccess.Add(1, new KeyValuePair<string, object?>(PrimitiveTypeTag, primitiveName));
    }

    /// <summary>
    ///     Records that a domain primitive rejected a value.
    /// </summary>
    /// <param name="primitiveName">The domain primitive.</param>
    /// <param name="error">Why.</param>
    public static void RecordValidationFailure(string primitiveName, PrimitiveError error)
    {
        if (ValidationFailure.Enabled)
        {
            ValidationFailure.Add(
                1,
                new KeyValuePair<string, object?>(PrimitiveTypeTag, primitiveName),
                new KeyValuePair<string, object?>(ErrorTypeTag, error.Code)
            );
        }

        if (!ActivitySource.HasListeners() || Activity.Current is not { } activity) return;
        activity.AddEvent(
            new(
                "domain_primitive.validation.failure",
                tags: new()
                {
                    { PrimitiveTypeTag, primitiveName },
                    { ErrorTypeTag, error.Code },
                    { "error_message", error.Message },
                }
            )
        );
    }

    private const string PrimitiveTypeTag = "primitive_type";
    private const string ErrorTypeTag = "error_type";

    private static readonly Counter<long> ValidationSuccess =
        Meter.CreateCounter<long>("domain_primitive.validation.success", description: "Number of values domain primitives accepted.");

    private static readonly Counter<long> ValidationFailure =
        Meter.CreateCounter<long>("domain_primitive.validation.failure", description: "Number of values domain primitives rejected.");

    private static readonly Counter<long> Creation =
        Meter.CreateCounter<long>("domain_primitive.creation", description: "Number of domain primitives created.");
}
