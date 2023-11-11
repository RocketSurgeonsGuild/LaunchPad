using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class CompositeValidator
{
    protected static readonly PropertyInfo RuleSetsExecutedProperty = typeof(ValidationResult)
        // ReSharper disable once NullableWarningSuppressionIsUsed
       .GetProperty(nameof(ValidationResult.RuleSetsExecuted), BindingFlags.Instance | BindingFlags.Public)!;
}

internal class CompositeValidator<T>(IEnumerable<IValidator> validators) : CompositeValidator, IValidator<T>
{
    public CascadeMode CascadeMode { get; set; } = CascadeMode.Continue;

    public ValidationResult Validate(T instance)
    {
        return Validate(new ValidationContext<T>(instance));
    }

    public Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = new())
    {
        return ValidateAsync(new ValidationContext<T>(instance), cancellation);
    }

    public ValidationResult Validate(IValidationContext context)
    {
        return validators
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
    }

    public async Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = default)
    {
        var tasks = new List<Task<ValidationResult>>();
        foreach (var validator in validators)
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

    public IValidatorDescriptor CreateDescriptor()
    {
        return new CompositeValidatorDescriptor(validators);
    }

    public bool CanValidateInstancesOfType(Type type)
    {
        return validators.Any(z => z.CanValidateInstancesOfType(type));
    }
}
