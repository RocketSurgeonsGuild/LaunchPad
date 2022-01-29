using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

[UsedImplicitly]
internal class ProblemDetailsValidator : AbstractValidator<ProblemDetails>
{
    public ProblemDetailsValidator()
    {
        RuleFor(x => x.Type).NotNull();
        RuleFor(x => x.Title).NotNull();
    }
}
