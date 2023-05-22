//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator/RocketController_Methods.cs
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
        public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords(Guid id, [Bind()][BindRequired][FromRoute] GetRocketLaunchRecords.Request request)
        {
            var result = Mediator.CreateStream(request with {Id = id}, HttpContext.RequestAborted);
            return result;
        }
    }
}
#nullable restore
