using FluentValidation;
using JetBrains.Annotations;

namespace Rocket.Surgery.SpaceShuttle.AspNetCore.Validation
{
    [UsedImplicitly]
    internal class FluentValidationProblemDetailsValidator : AbstractValidator<FluentValidationProblemDetails>
    {
        public FluentValidationProblemDetailsValidator()
        {
            Include(new ProblemDetailsValidator());
            RuleFor(x => x.Rules).NotNull();
            RuleFor(x => x.Errors).NotNull();
        }
    }
}