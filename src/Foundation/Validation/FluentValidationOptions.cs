using FluentValidation;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

/// <summary>
///     This class enables fluent validators to be used for options validations!
/// </summary>
/// <typeparam name="T"></typeparam>
internal class FluentValidationOptions<T> : IValidateOptions<T>
    where T : class
{
    private readonly ValidationHealthCheckResults? _healthCheckResults;
    private readonly IValidator<T>? _validator;

    public FluentValidationOptions(
        ValidationHealthCheckResults? healthCheckResults = null,
        IValidator<T>? validator = null
    )
    {
        _healthCheckResults = healthCheckResults;
        _validator = validator;
    }

    public ValidateOptionsResult Validate(string name, T options)
    {
        if (_validator == null) return ValidateOptionsResult.Skip;

        var result = _validator.Validate(options);
        _healthCheckResults?.AddResult(typeof(T).Name, name, result);
        if (_healthCheckResults is not null) return ValidateOptionsResult.Skip;
        if (result.IsValid) return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(
            new[] { $"Failure while validating {typeof(T).Name}{( name == Options.DefaultName ? "" : $" (Name: {name})" )}." }
               .Concat(result.Errors.Select(z => z.ToString()))
        );
    }
}
