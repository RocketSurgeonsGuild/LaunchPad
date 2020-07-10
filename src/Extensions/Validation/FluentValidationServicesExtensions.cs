using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    /// <summary>
    /// Fluent validations
    /// </summary>
    [PublicAPI]
    public static class FluentValidationServicesExtensions
    {

        /// <summary>
        /// Extension to add fluent validation validators from all the defined types of an assembly (instead of only public types)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddConventionValidatorsFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (services.Any(z => z.ImplementationType == typeof(ValidatorFactory)))
                return services;

            foreach (var item in new AssemblyScanner(assemblies.SelectMany(z => z.DefinedTypes).Select(x => x.AsType())))
            {
                services.TryAddEnumerable(new ServiceDescriptor(item.InterfaceType, item.ValidatorType, lifetime));
            }

            services.TryAddSingleton<IValidatorFactory, ValidatorFactory>();

            return services;
        }
    }
}