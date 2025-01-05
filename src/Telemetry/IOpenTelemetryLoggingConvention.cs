using Microsoft.Extensions.Configuration;


using OpenTelemetry.Logs;


using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///   IOpenTelemetryLoggingConvention
/// </summary>
[PublicAPI]
public interface IOpenTelemetryLoggingConvention : IConvention
{
    /// <summary>
    ///   Register the OpenTelemetry logging
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IConfiguration configuration, LoggerProviderBuilder builder);
}
