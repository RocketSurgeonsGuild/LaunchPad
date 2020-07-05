using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.Restful.Composition
{
    public interface IValidationActionResultFactory
    {
        ActionResult CreateActionResult(ValidationProblemDetails problemDetails);
        int StatusCode { get; }
    }
}