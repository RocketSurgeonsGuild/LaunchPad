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
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial IAsyncEnumerable<RocketModel> ListRockets([FromQuery] ListRockets.Request model)
        {
            var result = Mediator.CreateStream(model, HttpContext.RequestAborted);
            return result;
        }

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
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial async Task<ActionResult> SaveRocket(Guid id, [Bind(), CustomizeValidator(Properties = "")] SaveRocket.Request request)
        {
            await Mediator.Send(request with {Id = id}, HttpContext.RequestAborted).ConfigureAwait(false);
            return new StatusCodeResult(204);
        }

        [ProducesDefaultResponseType]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial async Task<ActionResult<RocketModel>> Save2Rocket(Guid id, [Bind("Sn"), CustomizeValidator(Properties = "Sn")] Save2Rocket.Request request)
        {
            request.Id = id;
            var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return new ObjectResult(result)
            {StatusCode = 200};
        }
    }
}
#nullable restore
