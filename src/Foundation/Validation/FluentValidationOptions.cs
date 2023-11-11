using FluentValidation;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.Foundation.Extensions;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

/// <summary>
///     This class enables fluent validators to be used for options validations!
/// </summary>
/// <typeparam name="T"></typeparam>
internal class FluentValidationOptions<T>(
    ValidationHealthCheckResults? healthCheckResults = null,
    IValidator<T>? validator = null
)
    : IValidateOptions<T>
    where T : class
{
    public virtual ValidateOptionsResult Validate(string? name, T options)
    {
        if (validator == null) return ValidateOptionsResult.Skip;

        var result = validator.Validate(options);
        healthCheckResults?.AddResult(typeof(T).GetNestedTypeName(), name ?? Options.DefaultName, result);
        if (result.IsValid) return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(
            new[] { $"Failure while validating {typeof(T).GetNestedTypeName()}{( name == Options.DefaultName ? "" : $" (Name: {name})" )}." }
               .Concat(result.Errors.Select(z => z.ToString()))
        );
    }
}
