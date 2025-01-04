using FluentValidation;

using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation.Extensions;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

/// <summary>
///     This class enables fluent validators to be used for options validations!
/// </summary>
/// <typeparam name="T"></typeparam>
internal class FluentValidationOptions<T>(IValidator<T>? validator = null)
    : IValidateOptions<T>
    where T : class
{
    public virtual ValidateOptionsResult Validate(string? name, T options)
    {
        if (validator is null) return ValidateOptionsResult.Skip;

        var result = validator.Validate(options);
        return result.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(
                new[] { $"Failure while validating {typeof(T).GetNestedTypeName()}{( name == Options.DefaultName ? "" : $" (Name: {name})" )}." }
                   .Concat(result.Errors.Select(z => z.ToString()))
            );
    }
}
