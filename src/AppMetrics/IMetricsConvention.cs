#if CONVENTIONS
using App.Metrics;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.AppMetrics
{
    /// <summary>
    /// IMetricsConvention
    /// Implements the <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="IConvention" />
    public interface IMetricsConvention : IConvention
    {
        /// <summary>
        /// Register metrics
        /// </summary>
        /// <param name="conventionContext"></param>
        /// <param name="configuration"></param>
        /// <param name="metricsBuilder"></param>
        void Register([NotNull] IConventionContext conventionContext, [NotNull] IConfiguration configuration, [NotNull] IMetricsBuilder metricsBuilder);
    }
}
#endif