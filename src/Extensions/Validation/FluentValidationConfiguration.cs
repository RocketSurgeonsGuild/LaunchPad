using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    /// <summary>
    /// Configuration settings for launch pad validation
    /// </summary>
    public class FluentValidationConfiguration
    {
        /// <summary>
        /// The default service lifetime
        /// </summary>
        public ServiceLifetime Lifetime { get; private set; } = ServiceLifetime.Singleton;
    }
}