using Microsoft.Extensions.Configuration;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     IOpenTelemetryMetricsConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface IOpenTelemetryMetricsConvention : IConvention
{
    /// <summary>
    ///     Register metrics
    /// </summary>
    /// <param name="conventionContext"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext conventionContext, IConfiguration configuration, MeterProviderBuilder builder);
}

/// <summary>
///     IOpenTelemetryTracingConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface IOpenTelemetryTracingConvention : IConvention
{
    /// <summary>
    ///     Register tracing
    /// </summary>
    /// <param name="conventionContext"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext conventionContext, IConfiguration configuration, TracerProviderBuilder builder);
}
