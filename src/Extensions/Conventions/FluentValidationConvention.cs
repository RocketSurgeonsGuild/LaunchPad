#if CONVENTIONS
using System;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Configuration;
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
    [AfterConvention(typeof(MediatRConvention))]
    public class FluentValidationConvention : IServiceConvention
    {
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

            AddLaunchPadValidationExtension.AddLaunchPadValidation(
                services,
                context
                   .AssemblyCandidateFinder
                   .GetCandidateAssemblies(nameof(global::FluentValidation)),
                context.Get<FluentValidationConfiguration>()
            );
        }
    }
}
#endif