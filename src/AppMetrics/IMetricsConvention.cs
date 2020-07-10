using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.AppMetrics
{
    /// <summary>
    /// IMetricsConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IMetricsConventionContext}" />
    public interface IMetricsConvention : IConvention<IMetricsConventionContext> { }
}