using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.EntityFramework
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
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddLaunchPadEntityFramework(this IServiceCollection services,
                                                                     IEnumerable<Assembly> assemblies)
        {
            services.AddSingleton<IOnModelCreating, SqliteDateTimeOffset>();
            services.AddSingleton<IOnModelCreating, ConfigureEntityTypes>();
            services.Scan(
                x => x
                   .FromAssemblies(assemblies)
                   .AddClasses(c => c.AssignableTo<IOnEntityCreating>())
                   .AsImplementedInterfaces()
                   .WithSingletonLifetime()
            );

            return services;
        }
    }
}