using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryAsyncConvention
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
/// <param name="cancellationToken"></param>
public delegate ValueTask OpenTelemetryAsyncConvention(IConventionContext conventionContext, IConfiguration configuration, IOpenTelemetryBuilder builder, CancellationToken cancellationToken);
