using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.NewtonsoftJson.Conventions;

[assembly: Convention(typeof(ValidationProblemDetailsNewtonsoftJsonConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.NewtonsoftJson.Conventions
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
        public void Register([NotNull] IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.AddValidationProblemDetailsNewtonsoftJson();
        }
    }
}