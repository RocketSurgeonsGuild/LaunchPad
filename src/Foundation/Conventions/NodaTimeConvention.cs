using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.TimeZones;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;
using System;

[assembly: Convention(typeof(NodaTimeConvention))]

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions
{
    /// <summary>
    /// NodaTimeConvention.
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    [LiveConvention]
    public class NodaTimeConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.AddSingleton<IClock>(SystemClock.Instance);
            services.AddSingleton<IDateTimeZoneProvider, DateTimeZoneCache>();
            services.AddSingleton<IDateTimeZoneSource>(TzdbDateTimeZoneSource.Default);
        }
    }
}