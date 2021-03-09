using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition
{
    public interface IValidationActionResultFactory
    {
        ActionResult CreateActionResult(ValidationProblemDetails problemDetails);
        int StatusCode { get; }
    }
}