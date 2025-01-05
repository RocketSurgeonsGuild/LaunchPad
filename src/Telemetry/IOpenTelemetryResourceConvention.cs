using Microsoft.Extensions.Configuration;

using OpenTelemetry.Resources;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     OpenTelemetryResourceConvention
/// </summary>
[PublicAPI]
public interface IOpenTelemetryResourceConvention : IConvention
{
    /// <summary>
    ///     Register the OpenTelemetry resource
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IConfiguration configuration, ResourceBuilder builder);
}
