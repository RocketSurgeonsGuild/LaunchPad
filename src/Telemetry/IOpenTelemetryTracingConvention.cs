using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     IMetricsConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface IOpenTelemetryTracingConvention : IConvention
{
    /// <summary>
    ///     Register metrics
    /// </summary>
    /// <param name="conventionContext"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext conventionContext, IConfiguration configuration, TracerProviderBuilder builder);
}
