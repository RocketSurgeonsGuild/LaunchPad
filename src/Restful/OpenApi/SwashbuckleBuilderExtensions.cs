using System;
using JetBrains.Annotations;
using Rocket.Surgery.AspNetCore.Swashbuckle;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
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