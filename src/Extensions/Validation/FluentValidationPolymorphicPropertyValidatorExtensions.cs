using System;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    /// <summary>
    /// FluentValidationPolymorphicPropertyValidatorExtensions.
    /// </summary>
    [PublicAPI]
    public static class FluentValidationPolymorphicPropertyValidatorExtensions
    {
        /// <summary>
        /// Uses the polymorphic validator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty">The type of the t property.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>IRuleBuilderOptions{T, TProperty}.</returns>
        public static IRuleBuilderOptions<T, TProperty> UsePolymorphicValidator<T, TProperty>(
            this IRuleBuilder<T, TProperty> builder,
            IServiceProvider serviceProvider
        )
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.SetValidator(
                ActivatorUtilities.CreateInstance<PolymorphicPropertyValidator<TProperty>>(serviceProvider)
            );
        }
    }
}