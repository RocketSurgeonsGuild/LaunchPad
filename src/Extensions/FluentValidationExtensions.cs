using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.SpaceShuttle.Extensions.Validation;


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
        public static IRuleBuilderOptions<T, TProperty> IsOneOf<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool caseSensitive, params string[] values)
            => ruleBuilder.SetValidator(new StringInValidator(values, caseSensitive));

        /// <summary>
        /// Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.
        /// </summary>
        /// <typeparam name="T">Type of Enum being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="values">The values to match against</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> IsOneOf<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params string[] values)
            => ruleBuilder.SetValidator(new StringInValidator(values, false));
    }
}