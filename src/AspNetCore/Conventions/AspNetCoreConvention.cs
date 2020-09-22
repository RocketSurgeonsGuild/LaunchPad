#if CONVENTIONS
using System;
using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

[assembly: Convention(typeof(AspNetCoreConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    /// <summary>
    /// Class MvcConvention.
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    public class AspNetCoreConvention : IServiceConvention
    {
        private readonly Action<FluentValidationMvcConfiguration>? _validatorConfiguration;

        /// <summary>
        /// Configure aspnet with some logical defaults
        /// </summary>
        public AspNetCoreConvention(Action<FluentValidationMvcConfiguration>? validatorConfiguration = null)
        {
            _validatorConfiguration = validatorConfiguration;
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// TODO Edit XML Comment Template for Register
        public void Register([NotNull] IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.AddLaunchPadFluentValidation(_validatorConfiguration);
        }
    }
}
#endif