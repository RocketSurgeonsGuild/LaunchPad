using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// Extensions used to pull in the default services for launchpad
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadServicesExtension
    {
        /// <summary>
        /// Adds the launchpad services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLaunchPad(this IServiceCollection services) => services
           .AddOptions()
           .AddLogging()
           .AddExecuteScopedServices();
    }
}