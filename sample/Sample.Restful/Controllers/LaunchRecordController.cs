using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public class LaunchRecordController : RestfulApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<IEnumerable<LaunchRecordModel>>> ListLaunchRecords()
    {
        return Send(new ListLaunchRecords.Request(), x => Ok(x));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<LaunchRecordModel>> GetLaunchRecord([BindRequired] [FromRoute] GetLaunchRecord.Request request)
    {
        return Send(request, x => Ok(x));
    }

    [HttpPost]
    public Task<ActionResult<CreateLaunchRecord.Response>> CreateLaunchRecord([BindRequired] [FromBody] CreateLaunchRecord.Request request)
    {
        return Send(
            request,
            x => CreatedAtAction(nameof(GetLaunchRecord), new { id = x.Id }, x)
        );
    }

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateLaunchRecord([BindRequired] [FromRoute] Guid id, [BindRequired] [FromBody] EditLaunchRecord.Model model)
    {
        return Send(new EditLaunchRecord.Request { Id = id }.With(model), NoContent);
    }

    [HttpDelete("{id:guid}")]
    public Task<ActionResult> RemoveLaunchRecord([BindRequired] [FromRoute] DeleteLaunchRecord.Request request)
    {
        return Send(request);
    }
}
