﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator/RocketController_ControllerMethods.cs
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
        public partial async Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord(Guid id, Guid launchId, [Bind()][BindRequired][FromRoute] GetRocketLaunchRecord.Request request)
        {
            var result = await Mediator.Send(request with { Id = id, LaunchId = launchId }, HttpContext.RequestAborted).ConfigureAwait(false);
            return new ObjectResult(result)
            {
                StatusCode = 200
            };
        }
    }
}
#nullable restore