using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.SpaceShuttle.AspNetCore.Validation
{
    [UsedImplicitly]
    internal class ProblemDetailsValidator : AbstractValidator<ProblemDetails>
    {
        public ProblemDetailsValidator()
        {
            RuleFor(x => x.Type).NotNull();
            RuleFor(x => x.Title).NotNull();
        }
    }
}