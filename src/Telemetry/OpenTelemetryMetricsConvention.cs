using Microsoft.Extensions.Configuration;
using OpenTelemetry.Metrics;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryMetricsConvention
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
public delegate void OpenTelemetryMetricsConvention(IConventionContext conventionContext, IConfiguration configuration, MeterProviderBuilder builder);
