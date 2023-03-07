using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryMetricsConvention
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
public delegate void OpenTelemetryConvention(IConventionContext conventionContext, IConfiguration configuration, OpenTelemetryBuilder builder);
