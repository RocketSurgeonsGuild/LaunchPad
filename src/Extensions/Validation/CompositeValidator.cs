using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    class CompositeValidator<T> : IValidator<T>
    {
        private readonly IEnumerable<IValidator> _validators;

        public CompositeValidator(IEnumerable<IValidator> validators) => _validators = validators;

        public ValidationResult Validate(T instance)
            => Validate(new ValidationContext<T>(instance));

        public Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = new CancellationToken())
            => ValidateAsync(new ValidationContext<T>(instance), cancellation);

        public ValidationResult Validate(IValidationContext context) => _validators
           .Select(z => z.Validate(context))
           .Aggregate(
                new ValidationResult(),
                (acc, result) =>
                {
                    var response = new ValidationResult(acc.Errors.Union(result.Errors).OrderBy(z => z.PropertyName));
                    RuleSetsExecutedProperty.SetValue(result, acc.RuleSetsExecuted?.Union(response.RuleSetsExecuted ?? Array.Empty<string>()).ToArray());
                    return response;
                }
            );

        public async Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = default)
        {
            var tasks = new List<Task<ValidationResult>>();
            foreach (var validator in _validators)
            {
                tasks.Add(validator.ValidateAsync(context, cancellation));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.Aggregate(
                new ValidationResult(),
                (acc, result) =>
                {
                    var response = new ValidationResult(acc.Errors.Union(result.Errors).OrderBy(z => z.PropertyName));
                    RuleSetsExecutedProperty.SetValue(result, acc.RuleSetsExecuted?.Union(response.RuleSetsExecuted ?? Array.Empty<string>()).ToArray());
                    return response;
                }
            );
        }

        public IValidatorDescriptor CreateDescriptor() => new CompositeValidatorDescriptor(_validators);

        public bool CanValidateInstancesOfType(Type type) => _validators.Any(z => z.CanValidateInstancesOfType(type));

        public CascadeMode CascadeMode { get; set; } = CascadeMode.Continue;

        private static readonly PropertyInfo RuleSetsExecutedProperty = typeof(ValidationResult)
           .GetProperty(nameof(ValidationResult.RuleSetsExecuted), BindingFlags.Instance | BindingFlags.Public)!;
    }
}