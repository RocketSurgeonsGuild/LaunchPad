using FluentValidation;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.Foundation.Extensions;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

/// <summary>
///     This class enables fluent validators to be used for options validations!
/// </summary>
/// <typeparam name="T"></typeparam>
internal class HealthCheckFluentValidationOptions<T> : FluentValidationOptions<T>
    where T : class
{
    private readonly ValidationHealthCheckResults _healthCheckResults;
    private readonly IValidator<T>? _validator;

    public HealthCheckFluentValidationOptions(
        ValidationHealthCheckResults healthCheckResults,
        IValidator<T>? validator = null
    ) : base(null /* null because we're adding results during the validate call here */, validator)
    {
        _healthCheckResults = healthCheckResults;
        _validator = validator;
    }

    public override ValidateOptionsResult Validate(string name, T options)
    {
        if (_validator == null) return ValidateOptionsResult.Skip;

        var result = _validator.Validate(options);
        _healthCheckResults.AddResult(typeof(T).GetNestedTypeName(), name, result);
        return _healthCheckResults.ApplicationHasStarted ? base.Validate(name, options) : ValidateOptionsResult.Skip;
    }
}
