using System;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.FluentValidation;
using Rocket.Surgery.Extensions.FluentValidation;

[assembly: Convention(typeof(FluentValidationConvention))]

namespace Rocket.Surgery.Conventions.FluentValidation
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
        }
    }
}