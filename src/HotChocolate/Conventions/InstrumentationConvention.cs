using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     InstrumentationConvention.
///     Implements <see cref="IOpenTelemetryMetricsConvention" /> and <see cref="IOpenTelemetryTracingConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class InstrumentationConvention : IOpenTelemetryTracingConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, TracerProviderBuilder builder)
    {
        builder.AddHotChocolateInstrumentation();
    }
}
