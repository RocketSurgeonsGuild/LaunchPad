using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     A validation error problem
/// </summary>
[PublicAPI]
public class FluentValidationProblemDetail
{
    /// <summary>
    ///     A validation error problem
    /// </summary>
    public FluentValidationProblemDetail(ValidationFailure validationFailure)
    {
        ArgumentNullException.ThrowIfNull(validationFailure);

        PropertyName = validationFailure.PropertyName;
        ErrorMessage = validationFailure.ErrorMessage;
        AttemptedValue = validationFailure.AttemptedValue;
        Severity = validationFailure.Severity;
        ErrorCode = validationFailure.ErrorCode;
    }

    /// <summary>The name of the property.</summary>
    [JsonPropertyName("propertyName")]
    public string PropertyName { get; set; }

    /// <summary>The error message</summary>
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }

    /// <summary>The property value that caused the failure.</summary>
    [JsonPropertyName("attemptedValue")]
    public object? AttemptedValue { get; set; }

    /// <summary>Custom severity level associated with the failure.</summary>
    [JsonPropertyName("severity")]
    public Severity Severity { get; set; }

    /// <summary>Gets or sets the error code.</summary>
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; }

    internal class Validator : AbstractValidator<FluentValidationProblemDetail>
    {
        public Validator()
        {
            RuleFor(x => x.PropertyName).NotNull();
            RuleFor(x => x.ErrorCode).NotNull();
            RuleFor(x => x.ErrorMessage).NotNull();
        }
    }
}
