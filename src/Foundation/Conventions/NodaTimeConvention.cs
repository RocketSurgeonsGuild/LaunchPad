using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public class NodaTimeConvention : IServiceConvention
    {
        private readonly FoundationOptions _options;

        /// <summary>
        /// Create the NodaTime convention
        /// </summary>
        /// <param name="options"></param>
        public NodaTimeConvention(FoundationOptions? options = null)
        {
            _options = options ?? new FoundationOptions();
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.TryAddSingleton<IClock>(SystemClock.Instance);
            services.TryAddSingleton<IDateTimeZoneProvider, DateTimeZoneCache>();
            services.TryAddSingleton(_options.DateTimeZoneSource);
        }
    }
}