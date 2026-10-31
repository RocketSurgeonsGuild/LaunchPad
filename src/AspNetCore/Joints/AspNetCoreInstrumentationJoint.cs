using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Adds the ASP.NET Core tracing and metrics instrumentation to OpenTelemetry.
///     Implements the <see cref="IOpenTelemetryJoint" />
/// </summary>
/// <remarks>
///     This joint takes the whole <see cref="IOpenTelemetryBuilder" /> because the tracer and meter joint interfaces
///     cannot both be implemented by one class, and the instrumentation needs both signals.
/// </remarks>
/// <seealso cref="IOpenTelemetryJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class AspNetCoreInstrumentationJoint : IOpenTelemetryJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IOpenTelemetryBuilder builder)
    {
        builder.WithTracing(b => b.AddAspNetCoreInstrumentation(options => options.RecordException = true));
        builder.WithMetrics(b => b.AddAspNetCoreInstrumentation());
    }
}
