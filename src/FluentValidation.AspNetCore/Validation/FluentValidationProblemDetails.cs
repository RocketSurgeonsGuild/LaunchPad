using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     Problem Details for Fluent Validation
/// </summary>
[PublicAPI]
public class FluentValidationProblemDetails : ValidationProblemDetails
{
    internal static FluentValidationProblemDetails From(ValidationProblemDetails validationProblemDetails, ValidationResult validationResult)
    {
        var result = new FluentValidationProblemDetails(validationResult)
        {
            Detail = validationProblemDetails.Detail,
            Instance = validationProblemDetails.Instance,
            Status = validationProblemDetails.Status,
            Title = validationProblemDetails.Title,
            Type = validationProblemDetails.Type,
        };
        foreach (var ext in validationProblemDetails.Extensions)
        {
            result.Extensions.TryAdd(ext.Key, ext.Value);
        }

        return result;
    }

    /// <summary>
    ///     Construct the Fluent Validation Problem Details
    /// </summary>
    public FluentValidationProblemDetails() : this(Array.Empty<ValidationFailure>()) { }

    /// <summary>
    ///     Build Fluent Validation Problem Details from a <see cref="ValidationResult" />
    /// </summary>
    /// <param name="result"></param>
    public FluentValidationProblemDetails(ValidationResult result) : this(result.Errors)
    {
        ArgumentNullException.ThrowIfNull(result);

        Rules = result.RuleSetsExecuted;
    }

    /// <summary>
    ///     Build Fluent Validation Problem Details from a <see cref="IEnumerable{T}" />
    /// </summary>
    /// <param name="errors"></param>
    public FluentValidationProblemDetails(IEnumerable<ValidationFailure> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        ValidationErrors = errors
                          .ToLookup(x => x.PropertyName)
                          .ToDictionary(z => z.Key, z => z.Select(item => new FluentValidationProblemDetail(item)).ToArray());
    }

    /// <summary>
    ///     Gets the validation errors associated with this instance of <see cref="FluentValidationProblemDetail" />.
    /// </summary>
    public IDictionary<string, FluentValidationProblemDetail[]> ValidationErrors { get; }

    /// <summary>
    ///     The rules run with the validation
    /// </summary>
    public IEnumerable<string> Rules { get; set; } = Array.Empty<string>();
}