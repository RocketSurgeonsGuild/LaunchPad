using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation
{
    /// <summary>
    /// PolymorphicPropertyValidator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class PolymorphicPropertyValidator<T, TProperty> : AsyncPropertyValidator<T, TProperty>
    {
        /// <inheritdoc />
        public override async Task<bool> IsValidAsync(ValidationContext<T> context, TProperty value, CancellationToken cancellation)
        {
            // bail out if the property is null
            if (value is not {}) return true;

            var factory = context.GetServiceProvider().GetRequiredService<IValidatorFactory>();
            var validator = factory.GetValidator(value.GetType());
            return validator is null || ( await validator.ValidateAsync(context, cancellation).ConfigureAwait(false) ).IsValid;
        }

        /// <inheritdoc />
        public override string Name { get; } = "PolymorphicPropertyValidator";
    }
}