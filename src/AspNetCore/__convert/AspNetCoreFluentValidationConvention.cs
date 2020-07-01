using System;
using System.Reflection;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.AspNetCore.FluentValidation;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.AspNetCore.FluentValidation;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.FluentValidation;

[assembly: Convention(typeof(AspNetCoreFluentValidationConvention))]

namespace Rocket.Surgery.Conventions.AspNetCore.FluentValidation
{
    /// <summary>
    /// AspNetCoreFluentValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class AspNetCoreFluentValidationConvention : IServiceConvention
    {
        private readonly FluentValidationMvcConfiguration _configuration;

        /// <summary>
        /// THe validation settings
        /// </summary>
        /// <param name="configuration"></param>
        public AspNetCoreFluentValidationConvention([CanBeNull] FluentValidationMvcConfiguration? configuration = null)
            => _configuration = configuration ?? new FluentValidationMvcConfiguration();

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

            context.Services
               .AddFluentValidationExtensions(_configuration);
        }
    }
}