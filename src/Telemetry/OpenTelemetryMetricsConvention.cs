using Microsoft.Extensions.Configuration;
using OpenTelemetry.Metrics;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryMetricsConvention
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
public delegate void OpenTelemetryMetricsConvention(IConventionContext context, IConfiguration configuration, MeterProviderBuilder builder);