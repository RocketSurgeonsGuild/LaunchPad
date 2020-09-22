using System;
using System.Collections.Generic;
using System.Reflection;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// Extensions used to pull in MediatR for launch pad
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadMediatRExtension
    {
        /// <summary>
        /// Adds the launchpad services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddLaunchPadMediatR(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            MediatRServiceConfiguration? configuration = null
        )
        {
            ServiceRegistrar.AddRequiredServices(services, configuration);
            ServiceRegistrar.AddMediatRClasses(services, assemblies);

            return services;
        }
    }
}