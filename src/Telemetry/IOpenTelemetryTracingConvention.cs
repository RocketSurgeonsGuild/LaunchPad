using Microsoft.Extensions.Configuration;

using OpenTelemetry.Trace;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///   OpenTelemetryResourceConvention
/// </summary>
public interface IOpenTelemetryTracingConvention : IConvention
{
    /// <summary>
    ///   Register the OpenTelemetry tracing
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IConfiguration configuration, TracerProviderBuilder builder);
}
