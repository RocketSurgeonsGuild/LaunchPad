using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    /// <summary>
    /// PolymorphicPropertyValidator.
    /// Implements the <see cref="NoopPropertyValidator" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <seealso cref="NoopPropertyValidator" />
    public class PolymorphicPropertyValidator<T, TProperty> : NoopPropertyValidator
    {
        /// <summary>
        /// Validates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable{ValidationFailure}.</returns>
        public override IEnumerable<ValidationFailure> Validate([NotNull] PropertyValidatorContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // bail out if the property is null
            if (context.PropertyValue == null || !( context.PropertyValue is TProperty value ))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var factory = context.GetServiceProvider().GetRequiredService<IValidatorFactory>();

            var validator = factory.GetValidator(value.GetType());
            if (context.ParentContext.IsChildCollectionContext)
            {
                return validator.Validate(ValidationContext<T>.GetFromNonGenericContext(context.ParentContext).CloneForChildValidator(value)).Errors;
            }

            var validationContext = new ValidationContext<TProperty>(
                value,
                PropertyChain.FromExpression(context.Rule.Expression),
                context.ParentContext.Selector
            );
            validationContext.SetServiceProvider(context.GetServiceProvider());
            return validator.Validate(validationContext).Errors;
        }

        /// <summary>
        /// validate as an asynchronous operation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellation">The cancellation.</param>
        /// <returns>Task{IEnumerable{ValidationFailure}}.</returns>
        public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(
            [NotNull] PropertyValidatorContext context,
            CancellationToken cancellation
        )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // bail out if the property is null
            if (context.PropertyValue == null || !( context.PropertyValue is TProperty value ))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var factory = context.GetServiceProvider().GetRequiredService<IValidatorFactory>();

            var validator = factory.GetValidator(value.GetType());
            if (context.ParentContext.IsChildCollectionContext)
            {
                return ( await validator.ValidateAsync(
                    ValidationContext<T>.GetFromNonGenericContext(context.ParentContext).CloneForChildValidator(value),
                    cancellation
                ).ConfigureAwait(false) ).Errors;
            }

            var validationContext = new ValidationContext<TProperty>(
                value,
                PropertyChain.FromExpression(context.Rule.Expression),
                context.ParentContext.Selector
            );
            validationContext.SetServiceProvider(context.GetServiceProvider());
            return ( await validator.ValidateAsync(validationContext, cancellation).ConfigureAwait(false) ).Errors;
        }
    }
}