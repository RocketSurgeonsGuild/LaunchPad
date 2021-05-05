using FluentValidation;
using FluentValidation.Validators;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation
{
    /// <summary>
    /// A validator that is used to verify the value is in a defined list without creating a specific enum
    /// </summary>
    [PublicAPI]
    public class StringInValidator<T, TProperty> : PropertyValidator<T, TProperty?>, IStringInValidator
        where TProperty : notnull
    {
        private readonly bool _caseSensitive;

        /// <inheritdoc />
        /// <param name="items">The string values to validate against</param>
        /// <param name="caseSensitive">Should character case be enforced?</param>
        public StringInValidator(string[] items, bool caseSensitive)
        {
            Values = items ?? throw new ArgumentNullException(nameof(items));
            _caseSensitive = caseSensitive;
        }


        /// <inheritdoc />
        protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' must include one of the following '{Values}'.";

        /// <summary>
        /// The values that being enforced.
        /// </summary>
        public IEnumerable<string> Values { get; }

        /// <inheritdoc />
        public override bool IsValid(ValidationContext<T> context, TProperty? value)
        {
            if (value == null) return true;
            var comparison = _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            var stringValue = value is string s ? s : value?.ToString();
            var result = Values.Any(n => n.Equals(stringValue, comparison));
            if (result)
            {
                return result;
            }

            context.MessageFormatter.AppendPropertyName(context.DisplayName);
            context.MessageFormatter.AppendArgument("Values", string.Join(", ", Values));
            return result;
        }

        /// <inheritdoc />
        public override string Name { get; } = "StringInValidator";
    }

    interface IStringInValidator
    {
        IEnumerable<string> Values { get; }
    }
}