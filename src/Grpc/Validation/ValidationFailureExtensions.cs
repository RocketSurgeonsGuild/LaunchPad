﻿using FluentValidation.Results;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation;

internal static class ValidationFailureExtensions
{
    public static IEnumerable<ValidationTrailers> ToValidationTrailers(this IList<ValidationFailure> failures)
    {
        return failures.Select(
            x => new ValidationTrailers
            {
                PropertyName = x.PropertyName,
                AttemptedValue = x.AttemptedValue,
                ErrorMessage = x.ErrorMessage
            }
        ).ToList();
    }
}
