using Microsoft.Extensions.Configuration;
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
public class InstrumentationConvention : IOpenTelemetryMetricsConvention, IOpenTelemetryTracingConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, MeterProviderBuilder builder)
    {
        builder.AddHttpClientInstrumentation();
    }

    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, TracerProviderBuilder builder)
    {
        builder.AddHttpClientInstrumentation(
            options =>
            {
                options.RecordException = true;
                options.SetHttpFlavor = true;
            }
        );
    }
}

/// <summary>
///     InstrumentationConvention.
///     Implements <see cref="IOpenTelemetryMetricsConvention" /> and <see cref="IOpenTelemetryTracingConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[LiveConvention]
public class EventCounterMetricsConvention : IOpenTelemetryMetricsConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, MeterProviderBuilder builder)
    {
        builder.AddEventCounterMetrics(
            options => options.RefreshIntervalSecs = configuration.GetValue("OpenTelemetry:Metrics:RefreshIntervalSecs", 30)
        );
    }
}

/// <summary>
///     InstrumentationConvention.
///     Implements <see cref="IOpenTelemetryMetricsConvention" /> and <see cref="IOpenTelemetryTracingConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[LiveConvention]
public class RuntimeMetricsConvention : IOpenTelemetryMetricsConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, MeterProviderBuilder builder)
    {
        builder.AddRuntimeInstrumentation();
    }
}
