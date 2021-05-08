using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;

[assembly: Convention(typeof(DefaultConvention))]

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions
{
    /// <summary>
    /// MediatRConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    public class DefaultConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services
               .AddOptions()
               .AddLogging()
               .AddExecuteScopedServices();
        }
    }
}