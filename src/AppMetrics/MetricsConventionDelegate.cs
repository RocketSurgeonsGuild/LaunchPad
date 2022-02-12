using App.Metrics;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.AppMetrics;

/// <summary>
///     Delegate MetricsConventionDelegate
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="metricsBuilder"></param>
public delegate void MetricsConvention(IConventionContext conventionContext, IConfiguration configuration, IMetricsBuilder metricsBuilder);
