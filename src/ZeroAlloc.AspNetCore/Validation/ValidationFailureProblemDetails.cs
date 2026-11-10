using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ZeroAlloc.Mediator.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.
/// </summary>
[PublicAPI]
public class ValidationFailureProblemDetails : ProblemDetails
{
    /// <summary>
    ///     Creates the problem details for the failures of a <see cref="ValidationFailedException" />.
    /// </summary>
    /// <param name="exception">The exception the mediator threw.</param>
    public static ValidationFailureProblemDetails From(ValidationFailedException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new()
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "One or more validation errors occurred.",
            Errors = exception
                    .Error.Failures
                    .GroupBy(failure => failure.PropertyName, StringComparer.Ordinal)
                    .ToDictionary(
                         group => group.Key,
                         group => group.Select(failure => new ValidationFailureDetail(failure.PropertyName, failure.ErrorMessage, failure.ErrorCode)).ToArray(),
                         StringComparer.Ordinal
                     ),
        };
    }

    /// <summary>
    ///     The failures, by property name.
    /// </summary>
    public IDictionary<string, ValidationFailureDetail[]> Errors { get; init; } = new Dictionary<string, ValidationFailureDetail[]>(StringComparer.Ordinal);
}

/// <summary>
///     A single validation failure.
/// </summary>
/// <param name="PropertyName">The property that failed.</param>
/// <param name="ErrorMessage">Why it failed.</param>
/// <param name="ErrorCode">The code of the rule that failed.</param>
[PublicAPI]
public record ValidationFailureDetail(string PropertyName, string ErrorMessage, string? ErrorCode);
