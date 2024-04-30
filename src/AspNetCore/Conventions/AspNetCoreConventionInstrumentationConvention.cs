using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     AspNetCoreConventionInstrumentationConvention.
///     Implements the <see cref="IOpenTelemetryConvention" />
/// </summary>
/// <seealso cref="IOpenTelemetryConvention" />
/// <seealso cref="IOpenTelemetryConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(AspNetCoreConvention))]
public class AspNetCoreConventionInstrumentationConvention : IOpenTelemetryConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IOpenTelemetryBuilder builder)
    {
        builder.WithTracing(b => b.AddAspNetCoreInstrumentation(options => options.RecordException = true));
        builder.WithMetrics(b => b.AddAspNetCoreInstrumentation());
    }
}