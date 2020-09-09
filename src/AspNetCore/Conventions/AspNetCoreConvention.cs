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
        private readonly ValidatorConfiguration? _validatorConfiguration;
        private readonly FluentValidationMvcConfiguration? _validationMvcConfiguration;

        /// <summary>
        /// Configure aspnet with some logical defaults
        /// </summary>
        /// <param name="validatorConfiguration"></param>
        /// <param name="validationMvcConfiguration"></param>
        public AspNetCoreConvention(
            ValidatorConfiguration? validatorConfiguration = null,
            FluentValidationMvcConfiguration? validationMvcConfiguration = null
            )
        {
            _validatorConfiguration = validatorConfiguration;
            _validationMvcConfiguration = validationMvcConfiguration;
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

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<NotFoundExceptionFilter>();
                options.Filters.Add<RequestFailedExceptionFilter>();
                options.Filters.Add<SerilogLoggingActionFilter>(0);
                options.Filters.Add<SerilogLoggingPageFilter>(0);
            });

            services.AddFluentValidationExtensions(_validatorConfiguration, _validationMvcConfiguration);
        }
    }
}
