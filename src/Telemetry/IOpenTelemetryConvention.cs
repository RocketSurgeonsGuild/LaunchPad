using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     IMetricsConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IOpenTelemetryConvention : IConvention
{
    /// <summary>
    ///     Register metrics
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IConfiguration configuration, IOpenTelemetryBuilder builder);
}
