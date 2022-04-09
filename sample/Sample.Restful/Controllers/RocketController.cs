using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public partial class RocketController : RestfulApiController
{
    /// <summary>
    ///     Search for rockets
    /// </summary>
    /// <param name="request">The search context</param>
    /// <returns></returns>
    [HttpGet]
    public partial IAsyncEnumerable<RocketModel> ListRockets(ListRockets.Request request);

    /// <summary>
    ///     Load details of a specific rocket
    /// </summary>
    /// <param name="request">The request context</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public partial Task<ActionResult<RocketModel>> GetRocket(GetRocket.Request request);

    /// <summary>
    ///     Create a new rocket
    /// </summary>
    /// <param name="request">The rocket details</param>
    /// <returns></returns>
    [HttpPost]
    [Created(nameof(GetRocket))]
    public partial Task<ActionResult<CreateRocket.Response>> CreateRocket(CreateRocket.Request request);

    /// <summary>
    ///     Update a given rocket
    /// </summary>
    /// <param name="id">The id of the rocket</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial Task<ActionResult<RocketModel>> EditRocket([BindRequired] [FromRoute] RocketId id, [BindRequired] [FromBody] EditRocket.Request model);

    /// <summary>
    ///     Update a given rocket
    /// </summary>
    /// <param name="id">The id of the rocket</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial Task<ActionResult<RocketModel>> PatchRocket([BindRequired] [FromRoute] RocketId id, [BindRequired] [FromBody] EditRocket.PatchRequest model);

    /// <summary>
    ///     Remove a rocket
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public partial Task<ActionResult> RemoveRocket([BindRequired] [FromRoute] DeleteRocket.Request request);

    /// <summary>
    ///     Get the launch records for a given rocket
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:guid}/launch-records")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords(GetRocketLaunchRecords.Request request);

    /// <summary>
    ///     Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:guid}/launch-records/{launchRecordId:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord(GetRocketLaunchRecord.Request request);
}
