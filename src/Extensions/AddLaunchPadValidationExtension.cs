using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.LaunchPad.Extensions.Validation;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// Add the fluent validation services
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadValidationExtension
    {
        /// <summary>
        /// Add the fluent validation services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddLaunchPadValidation(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            FluentValidationConfiguration? configuration = null
        )
        {
            var lifetime = configuration?.Lifetime ?? ServiceLifetime.Singleton;
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in new AssemblyScanner(assemblies.SelectMany(z => z.DefinedTypes).Select(x => x.AsType())))
            {
                services.TryAddEnumerable(new ServiceDescriptor(item.InterfaceType, item.ValidatorType, lifetime));
            }

            services.TryAddSingleton<IValidatorFactory, ValidatorFactory>();
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), lifetime));

            return services;
        }
    }
}