using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Hosting.Telemetry;

/// <summary>
///     Delegate OpenTelemetryMetricsConvention
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
public delegate void OpenTelemetryConvention(IConventionContext conventionContext, IConfiguration configuration, OpenTelemetryBuilder builder);
