//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator/Input3_RocketController_ControllerMethods.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
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
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial async Task<ActionResult> SaveRocket(Guid id, [Bind()][BindRequired][FromBody] SaveRocket.Request request)
        {
            await Mediator.Send(request with { Id = id }, HttpContext.RequestAborted).ConfigureAwait(false);
            return new StatusCodeResult(204);
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
