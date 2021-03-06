using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using System;
using System.Linq;

[assembly: Convention(typeof(FluentValidationConvention))]

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [AfterConvention(typeof(MediatRConvention))]
    public class FluentValidationConvention : IServiceConvention
    {
        private readonly FoundationOptions _options;

        /// <summary>
        /// Create the fluent validation convention
        /// </summary>
        /// <param name="options"></param>
        public FluentValidationConvention(FoundationOptions? options = null)
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

            var assemblies = context
               .AssemblyCandidateFinder
               .GetCandidateAssemblies("FluentValidation");
            foreach (var item in new AssemblyScanner(assemblies.SelectMany(z => z.DefinedTypes).Select(x => x.AsType())))
            {
                services.TryAddEnumerable(ServiceDescriptor.Describe(item.InterfaceType, item.ValidatorType, ServiceLifetime.Singleton));
            }

            if (services.FirstOrDefault(z => z.ServiceType == typeof(IValidatorFactory)) is { } s && s.Lifetime != ServiceLifetime.Singleton)
            {
                services.Remove(s);
            }
            services.TryAdd(ServiceDescriptor.Describe(typeof(IValidatorFactory), typeof(ValidatorFactory), ServiceLifetime.Singleton));
            services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), _options.MediatorLifetime));
        }
    }
}