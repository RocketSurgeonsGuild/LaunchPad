using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.TimeZones;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// Extensions used for NodaTime and LaunchPad
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadNodaTimeExtension
    {
        /// <summary>
        /// Adds the launchpad services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="clock"></param>
        /// <param name="dateTimeZoneSource"></param>
        /// <returns></returns>
        public static IServiceCollection AddLaunchPadNodaTime(this IServiceCollection services, IClock? clock = null, IDateTimeZoneSource? dateTimeZoneSource = null)
        {
            services.AddSingleton(clock?? SystemClock.Instance);
            services.AddSingleton<IDateTimeZoneProvider, DateTimeZoneCache>();
            services.AddSingleton(dateTimeZoneSource ?? TzdbDateTimeZoneSource.Default);

            return services;
        }
    }
}