#if CONVENTIONS
namespace Rocket.Surgery.LaunchPad.AppMetrics
{
    /// <summary>
    /// Options for configuration Metrics
    /// </summary>
    public class ConventionMetricsOptions
    {
        /// <summary>
        /// Use the default metrics configuration from App.Metrics
        /// </summary>
        public bool UseDefaults { get; set; } = true;
    }
}
#endif