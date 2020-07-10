using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Validators;
using JetBrains.Annotations;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    /// <summary>
    /// A validator that is used to verify the value is in a defined list without creating a specific enum
    /// </summary>
    [PublicAPI]
    public class StringInValidator : PropertyValidator
    {
        private readonly bool _caseSensitive;

        /// <inheritdoc />
        /// <param name="items">The string values to validate against</param>
        /// <param name="caseSensitive">Should character case be enforced?</param>
        public StringInValidator(string[] items, bool caseSensitive) : base("'{PropertyName}' must include one of the following '{Values}'.")
        {
            Values = items ?? throw new ArgumentNullException(nameof(items));
            _caseSensitive = caseSensitive;
        }


        /// <inheritdoc />
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;

            var value = context.PropertyValue.ToString();
            var comparison = _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            return Values.Any(n => n.Equals(value, comparison));
        }

        /// <summary>
        /// The values that being enforced.
        /// </summary>
        public IEnumerable<string> Values { get; }


        /// <inheritdoc />
        protected override void PrepareMessageFormatterForValidationError(
            PropertyValidatorContext context)
        {
            context.MessageFormatter.AppendPropertyName(context.DisplayName);
            context.MessageFormatter.AppendArgument("Values", string.Join(", ", Values));
        }
    }
}