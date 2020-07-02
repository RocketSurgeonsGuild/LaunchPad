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

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    /// <summary>
    /// PolymorphicPropertyValidator.
    /// Implements the <see cref="NoopPropertyValidator" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NoopPropertyValidator" />
    public class PolymorphicPropertyValidator<T> : NoopPropertyValidator
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicPropertyValidator{T}" /> class.
        /// </summary>
        /// <param name="validatorFactory">The validator factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        internal PolymorphicPropertyValidator(IValidatorFactory validatorFactory, IServiceProvider serviceProvider)
        {
            _validatorFactory = validatorFactory;
            _serviceProvider = serviceProvider;
        }

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
            if (context.PropertyValue == null || !( context.PropertyValue is T value ))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var validator = _validatorFactory.GetValidator(value.GetType());
            if (context.ParentContext.IsChildCollectionContext)
            {
                return validator.Validate(context.ParentContext.CloneForChildValidator(value)).Errors;
            }

            var validationContext = new ValidationContext<T>(
                value,
                PropertyChain.FromExpression(context.Rule.Expression),
                context.ParentContext.Selector
            );
            validationContext.SetServiceProvider(_serviceProvider);
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
            if (context.PropertyValue == null || !( context.PropertyValue is T value ))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var validator = _validatorFactory.GetValidator(value.GetType());
            if (context.ParentContext.IsChildCollectionContext)
            {
                return ( await validator.ValidateAsync(
                    context.ParentContext.CloneForChildValidator(value),
                    cancellation
                ).ConfigureAwait(false) ).Errors;
            }

            var validationContext = new ValidationContext<T>(
                value,
                PropertyChain.FromExpression(context.Rule.Expression),
                context.ParentContext.Selector
            );
            validationContext.SetServiceProvider(_serviceProvider);
            return ( await validator.ValidateAsync(validationContext, cancellation).ConfigureAwait(false) ).Errors;
        }
    }
}