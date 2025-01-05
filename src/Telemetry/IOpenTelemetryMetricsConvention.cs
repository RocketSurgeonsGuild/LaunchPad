using Microsoft.Extensions.Configuration;

using OpenTelemetry.Metrics;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///   IOpenTelemetryMetricsConvention
/// </summary>
[PublicAPI]
public interface IOpenTelemetryMetricsConvention : IConvention
{
    /// <summary>
    ///   Register the OpenTelemetry metrics
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IConfiguration configuration, MeterProviderBuilder builder);
}
