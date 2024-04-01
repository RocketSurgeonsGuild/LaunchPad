//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator/RocketController_ControllerMethods.cs
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
    }
}
#nullable restore
