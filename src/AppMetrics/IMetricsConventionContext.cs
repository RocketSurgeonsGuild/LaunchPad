using App.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.AppMetrics;

/// <summary>
///     IMetricsConventionContext
///     Implements the <see cref="IConventionContext" />
/// </summary>
/// <seealso cref="IConventionContext" />
public interface IMetricsConventionContext : IConventionContext
{
    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    /// <value>The configuration.</value>
    IConfiguration Configuration { get; }

    /// <summary>
    ///     Gets the metrics builder.
    /// </summary>
    /// <value>The metrics builder.</value>
    IMetricsBuilder MetricsBuilder { get; }

    /// <summary>
    ///     The environment that this convention is running
    ///     Based on IHostEnvironment / IHostingEnvironment
    /// </summary>
    /// <value>The environment.</value>
    IHostEnvironment Environment { get; }
}
