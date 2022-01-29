using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

[UsedImplicitly]
internal class ValidationProblemDetailsValidator : AbstractValidator<ValidationProblemDetails>
{
    public ValidationProblemDetailsValidator()
    {
        Include(new ProblemDetailsValidator());
        RuleFor(x => x.Errors).NotNull();
    }
}

// metrics
// health checks
// versioning
