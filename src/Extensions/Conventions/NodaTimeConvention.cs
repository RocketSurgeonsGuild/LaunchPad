#if CONVENTIONS
using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.TimeZones;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Extensions.Conventions;

[assembly: Convention(typeof(NodaTimeConvention))]

namespace Rocket.Surgery.LaunchPad.Extensions.Conventions
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

            services.AddLaunchPadNodaTime(context.Get<IClock>(), context.Get<IDateTimeZoneSource>());
        }
    }
}
#endif