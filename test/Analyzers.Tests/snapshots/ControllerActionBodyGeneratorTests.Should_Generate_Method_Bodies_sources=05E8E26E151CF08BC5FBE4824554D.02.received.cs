﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers\Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator\RocketController_Methods.cs
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
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(FluentValidationProblemDetails), 422)]
        public partial async Task<ActionResult> GetRocketLaunchRecords(Guid id, [Bind(), CustomizeValidator(Properties = "")][BindRequired][FromRoute] GetRocketLaunchRecords.Request request)
        {
            var result = await Mediator.Send(request with {Id = id}, HttpContext.RequestAborted).ConfigureAwait(false);
            return new StatusCodeResult(204);
        }
    }
}
#nullable restore
