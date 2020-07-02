using System;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.SpaceShuttle.Extensions;
using Rocket.Surgery.SpaceShuttle.Restful.Conventions;

namespace Rocket.Surgery.SpaceShuttle.Restful.OpenApi
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