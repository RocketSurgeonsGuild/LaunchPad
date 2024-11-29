using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryAsyncConvention
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask OpenTelemetryAsyncConvention(
    IConventionContext context,
    IConfiguration configuration,
    IOpenTelemetryBuilder builder,
    CancellationToken cancellationToken
);
