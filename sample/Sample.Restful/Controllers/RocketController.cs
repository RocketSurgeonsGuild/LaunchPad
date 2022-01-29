using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public class RocketController : RestfulApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<IEnumerable<RocketModel>>> ListRockets()
    {
        return Send(new ListRockets.Request(), x => Ok(x));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request)
    {
        return Send(request, x => Ok(x));
    }

    [HttpPost]
    public Task<ActionResult<CreateRocket.Response>> CreateRocket([BindRequired] [FromBody] CreateRocket.Request request)
    {
        return Send(
            request,
            x => CreatedAtAction(nameof(GetRocket), new { id = x.Id }, x)
        );
    }

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateRocket([BindRequired] Guid id, [BindRequired] [FromBody] EditRocket.Model model)
    {
        return Send(new EditRocket.Request { Id = id }.With(model), NoContent);
    }

    [HttpDelete("{id:guid}")]
    public Task<ActionResult> RemoveRocket([BindRequired] [FromRoute] DeleteRocket.Request request)
    {
        return Send(request);
    }
}
