using System;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Extensions.Conventions;
using Rocket.Surgery.LaunchPad.Extensions.Validation;

[assembly: Convention(typeof(FluentValidationConvention))]

namespace Rocket.Surgery.LaunchPad.Extensions.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    public class FluentValidationConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register([NotNull] IServiceConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Services.AddConventionValidatorsFromAssemblies(
                context
                   .AssemblyCandidateFinder
                   .GetCandidateAssemblies("FluentValidation")
            );

            var serviceConfig = context.GetOrAdd(() => new MediatRServiceConfiguration());
            context.Services.TryAddEnumerable(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>),
                    typeof(ValidationPipelineBehavior<,>),
                    serviceConfig.Lifetime
                )
            );
        }
    }
}