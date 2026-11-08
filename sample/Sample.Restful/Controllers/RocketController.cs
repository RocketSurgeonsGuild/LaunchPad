using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public class RocketController : RestfulApiController
{
    private IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    ///     Search for rockets
    /// </summary>
    /// <param name="request">The search context</param>
    /// <returns></returns>
    [HttpGet]
    public IAsyncEnumerable<RocketModel> ListRockets([FromQuery] ListRockets.Request request) => Mediator.CreateStream(request, HttpContext.RequestAborted);

    /// <summary>
    ///     Load details of a specific rocket
    /// </summary>
    /// <param name="request">The request context</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RocketModel>> GetRocket([BindRequired][FromRoute] GetRocket.Request request)
    {
        var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    ///     Create a new rocket
    /// </summary>
    /// <param name="request">The rocket details</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CreateRocket.Response>> CreateRocket([BindRequired][FromBody] CreateRocket.Request request)
    {
        var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetRocket), new { id = result.Id }, result);
    }

    /// <summary>
    ///     Update a given rocket
    /// </summary>
    /// <param name="id">The id of the rocket</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public async Task<ActionResult<RocketModel>> EditRocket(RocketId id, [Bind("SerialNumber", "Type")] EditRocket.Request model)
    {
        var result = await Mediator.Send(model with { Id = id }, HttpContext.RequestAborted).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    ///     Update a given rocket
    /// </summary>
    /// <param name="id">The id of the rocket</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public async Task<ActionResult<RocketModel>> PatchRocket(RocketId id, [Bind("SerialNumber", "Type")] EditRocket.PatchRequest model)
    {
        var result = await Mediator.Send(model with { Id = id }, HttpContext.RequestAborted).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    ///     Remove a rocket
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveRocket(DeleteRocket.Request request)
    {
        await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    ///     Get the launch records for a given rocket
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:guid}/launch-records")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords(RocketId id, [Bind()][BindRequired][FromRoute] GetRocketLaunchRecords.Request request) => Mediator.CreateStream(request with { Id = id }, HttpContext.RequestAborted);

    /// <summary>
    ///     Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:guid}/launch-records/{launchRecordId:guid}")]
    public async Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord(RocketId id, LaunchRecordId launchRecordId, [Bind()][BindRequired][FromRoute] GetRocketLaunchRecord.Request request)
    {
        var result = await Mediator.Send(request with { Id = id, LaunchRecordId = launchRecordId }, HttpContext.RequestAborted).ConfigureAwait(false);
        return result;
    }
}
