using System;
using System.Linq;
using JetBrains.Annotations;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// MediatRServicesExtensions.
    /// </summary>
    [PublicAPI]
    public static class MediatRServicesExtensions
    {
        /// <summary>
        /// Uses MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="services"></param>
        /// <returns>IServiceConventionContext.</returns>
        public static IConventionContext UseMediatR([NotNull] this IConventionContext builder, IServiceCollection services)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var serviceConfig = builder.GetOrAdd(() => new MediatRServiceConfiguration());
            return UseMediatR(builder, services, serviceConfig);
        }

        /// <summary>
        /// Uses MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="services"></param>
        /// <param name="serviceConfig">The MediatR service configuration.</param>
        /// <returns>IServiceConventionContext.</returns>
        public static IConventionContext UseMediatR(this IConventionContext builder, IServiceCollection services, MediatRServiceConfiguration serviceConfig)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (serviceConfig is null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            if (services.Any(z => z.ServiceType == typeof(IMediator))) return builder;

            builder.Set(serviceConfig);
            var assemblies = builder.AssemblyCandidateFinder
               .GetCandidateAssemblies(nameof(MediatR)).ToArray();

            ServiceRegistrar.AddRequiredServices(services, serviceConfig);
            ServiceRegistrar.AddMediatRClasses(services, assemblies);
            return builder;
        }
    }
}