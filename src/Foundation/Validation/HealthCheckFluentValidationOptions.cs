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
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    : FluentValidationOptions<T>(null, validator)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
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
