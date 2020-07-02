using System;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Extensions;
using Rocket.Surgery.LaunchPad.Restful.Conventions;

namespace Rocket.Surgery.LaunchPad.Restful.OpenApi
{
    /// <summary>
    /// FluentValidationHostBuilderExtensions.
    /// </summary>
    [PublicAPI]
    public static class SwashbuckleBuilderExtensions
    {
        /// <summary>
        /// Adds fluent validation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseSwashbuckle(this IConventionHostBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.UseFluentValidation();
            builder.Scanner.PrependConvention<SwashbuckleConvention>();
            return builder;
        }
    }
}