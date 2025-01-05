using Microsoft.Extensions.Configuration;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry.Conventions;

/// <summary>
///     InstrumentationConvention.
///     Implements <see cref="IOpenTelemetryConvention" /> and <see cref="IOpenTelemetryConvention" />
/// </summary>
/// <seealso cref="IOpenTelemetryConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class InstrumentationConvention : IOpenTelemetryConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IOpenTelemetryBuilder builder)
    {
        builder.WithTracing(b => b.AddHttpClientInstrumentation(x => x.RecordException = true));
        builder.WithMetrics(
            b => b
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
        );
    }
}
