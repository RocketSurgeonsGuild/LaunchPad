using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.AspNetCore.FluentValidation.NewtonsoftJson;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.AspNetCore.FluentValidation.NewtonsoftJson;
using Rocket.Surgery.Conventions.DependencyInjection;

[assembly: Convention(typeof(ValidationProblemDetailsNewtonsoftJsonConvention))]

namespace Rocket.Surgery.Conventions.AspNetCore.FluentValidation.NewtonsoftJson
{
    /// <summary>
    /// ValidationProblemDetailsNewtonsoftJsonConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    public class ValidationProblemDetailsNewtonsoftJsonConvention : IServiceConvention
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

            context.Services.AddValidationProblemDetailsNewtonsoftJson();
        }
    }
}