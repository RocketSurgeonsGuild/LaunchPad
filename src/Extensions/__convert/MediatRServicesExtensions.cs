using System;
using System.Linq;
using JetBrains.Annotations;
using MediatR;
using MediatR.Registration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
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
        /// <returns>IServiceConventionContext.</returns>
        public static IServiceConventionContext UseMediatR([NotNull] this IServiceConventionContext builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var serviceConfig = builder.GetOrAdd(() => new MediatRServiceConfiguration());
            return UseMediatR(builder, serviceConfig);
        }

        /// <summary>
        /// Uses MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="serviceConfig">The MediatR service configuration.</param>
        /// <returns>IServiceConventionContext.</returns>
        public static IServiceConventionContext UseMediatR(
            this IServiceConventionContext builder,
            MediatRServiceConfiguration serviceConfig
        )
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (serviceConfig is null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            if (builder.Services.Any(z => z.ServiceType == typeof(IMediator))) return builder;

            builder.Set(serviceConfig);
            var assemblies = builder.AssemblyCandidateFinder
               .GetCandidateAssemblies(nameof(MediatR)).ToArray();

            ServiceRegistrar.AddRequiredServices(builder.Services, serviceConfig);
            ServiceRegistrar.AddMediatRClasses(builder.Services, assemblies);
            return builder;
        }
    }
}