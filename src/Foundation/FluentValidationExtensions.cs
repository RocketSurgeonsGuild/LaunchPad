using JetBrains.Annotations;
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using System;

// ReSharper disable once CheckNamespace
namespace FluentValidation
{
    /// <summary>
    /// Fluent validations
    /// </summary>
    [PublicAPI]
    public static class FluentValidationExtensions
    {
        /// <summary>
        /// Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.
        /// </summary>
        /// <typeparam name="T">Type of Enum being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="caseSensitive">If the comparison between the string and the enum names should be case sensitive</param>
        /// <param name="values">The values to match against</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty?> IsOneOf<T, TProperty>(
            this IRuleBuilder<T, TProperty?> ruleBuilder,
            bool caseSensitive,
            params string[] values
        ) where TProperty : notnull => ruleBuilder.SetValidator(new StringInValidator<T, TProperty>(values, caseSensitive));

        /// <summary>
        /// Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.
        /// </summary>
        /// <typeparam name="T">Type of Enum being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="values">The values to match against</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> IsOneOf<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            params string[] values
        ) where TProperty : notnull
            => ruleBuilder.SetValidator(new StringInValidator<T, TProperty>(values, false));

        /// <summary>
        /// Uses the polymorphic validator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty">The type of the t property.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>IRuleBuilderOptions{T, TProperty}.</returns>
        public static IRuleBuilderOptions<T, TProperty> UsePolymorphicValidator<T, TProperty>(
            this IRuleBuilder<T, TProperty> builder
        )
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.SetAsyncValidator(new PolymorphicPropertyValidator<T, TProperty>());
        }
    }
}