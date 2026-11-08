using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using ZeroAlloc.Mediator;

namespace Sample.Classic.Restful.Controllers;

[Route("[controller]")]
public class LaunchRecordController : RestfulApiController
{
    private IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    ///     Search for launch records
    /// </summary>
    /// <param name="request">The search context</param>
    /// <returns></returns>
    [HttpGet]
    public IAsyncEnumerable<LaunchRecordModel> ListLaunchRecords([FromQuery] ListLaunchRecords.Request request) => Mediator.CreateStream(request, HttpContext.RequestAborted);

    /// <summary>
    ///     Load details of a specific launch record
    /// </summary>
    /// <param name="request">The request context</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LaunchRecordModel>> GetLaunchRecord([BindRequired][FromRoute] GetLaunchRecord.Request request)
    {
        var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    ///     Create a new launch record
    /// </summary>
    /// <param name="request">The launch record details</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CreateLaunchRecord.Response>> CreateLaunchRecord([BindRequired][FromBody] CreateLaunchRecord.Request request)
    {
        var result = await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetLaunchRecord), new { id = result.Id }, result);
    }

    /// <summary>
    ///     Update a given launch record
    /// </summary>
    /// <param name="id">The id of the launch record</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public async Task<ActionResult> EditLaunchRecord(LaunchRecordId id, [Bind("Partner", "Payload", "PayloadWeightKg", "ActualLaunchDate", "ScheduledLaunchDate", "RocketId")][BindRequired][FromBody] EditLaunchRecord.Request model)
    {
        await Mediator.Send(model with { Id = id }, HttpContext.RequestAborted).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    ///     Update a given launch record
    /// </summary>
    /// <param name="id">The id of the launch record</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public async Task<ActionResult> PatchLaunchRecord(LaunchRecordId id, [Bind("Partner", "Payload", "PayloadWeightKg", "ActualLaunchDate", "ScheduledLaunchDate", "RocketId")] EditLaunchRecord.PatchRequest model)
    {
        await Mediator.Send(model with { Id = id }, HttpContext.RequestAborted).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    ///     Remove a launch record
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteLaunchRecord([BindRequired][FromRoute] DeleteLaunchRecord.Request request)
    {
        await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return NoContent();
    }
}
