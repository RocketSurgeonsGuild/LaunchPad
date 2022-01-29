using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     Problem Details for Fluent Validation
/// </summary>
[PublicAPI]
public class FluentValidationProblemDetails : ValidationProblemDetails
{
    /// <summary>
    ///     Construct the Fluent Validation Problem Details
    /// </summary>
    public FluentValidationProblemDetails() : this(Array.Empty<ValidationFailure>())
    {
    }

    /// <summary>
    ///     Build Fluent Validation Problem Details from a <see cref="ValidationResult" />
    /// </summary>
    /// <param name="result"></param>
    public FluentValidationProblemDetails([NotNull] ValidationResult result) : this(result.Errors)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        Rules = result.RuleSetsExecuted;
    }

    /// <summary>
    ///     Build Fluent Validation Problem Details from a <see cref="IEnumerable{T}" />
    /// </summary>
    /// <param name="errors"></param>
    public FluentValidationProblemDetails([NotNull] IEnumerable<ValidationFailure> errors)
    {
        if (errors == null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

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
