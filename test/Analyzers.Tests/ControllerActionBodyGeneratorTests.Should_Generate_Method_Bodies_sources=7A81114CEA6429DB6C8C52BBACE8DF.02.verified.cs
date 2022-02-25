//HintName: Rocket.Surgery.LaunchPad.Analyzers\Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator\RocketController_Methods.cs
#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;
using FluentValidation.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

namespace MyNamespace.Controllers
{
    public partial class RocketController
    {
        [ProducesDefaultResponseType]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial async Task<ActionResult<RocketModel>> GetRocket(GetRocket.Request request)
        {
            var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return new ObjectResult(result)
            {StatusCode = 200};
        }

        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial async Task<ActionResult<CreateRocket.Response>> CreateRocket([BindRequired][FromBody] CreateRocket.Request request)
        {
            var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return new AcceptedAtActionResult("GetRocket", null, new
            {
            id = result.Id
            }

            , result);
        }
    }
}
#nullable restore
