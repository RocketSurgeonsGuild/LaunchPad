using FluentValidation;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.Foundation.Extensions;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

/// <summary>
///     This class enables fluent validators to be used for options validations!
/// </summary>
/// <typeparam name="T"></typeparam>
internal class HealthCheckFluentValidationOptions<T>(
    ValidationHealthCheckResults healthCheckResults,
    IValidator<T>? validator = null
)
    : FluentValidationOptions<T>(null, validator)
    where T : class
{
    /* null because we're adding results during the validate call here */

    public override ValidateOptionsResult Validate(string? name, T options)
    {
        if (validator == null) return ValidateOptionsResult.Skip;

        var result = validator.Validate(options);
        healthCheckResults.AddResult(typeof(T).GetNestedTypeName(), name ?? Options.DefaultName, result);
        return healthCheckResults.ApplicationHasStarted ? base.Validate(name, options) : ValidateOptionsResult.Skip;
    }
}
