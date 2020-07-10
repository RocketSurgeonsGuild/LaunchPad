using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.Restful.Composition
{
    public class UnprocessableEntityActionResultFactory : IValidationActionResultFactory
    {
        public ActionResult CreateActionResult(ValidationProblemDetails problemDetails)
        {
            problemDetails.Status = StatusCode;
            return new UnprocessableEntityObjectResult(problemDetails);
        }

        public int StatusCode { get; } = StatusCodes.Status422UnprocessableEntity;
    }
}