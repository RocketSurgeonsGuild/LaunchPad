using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public partial class LaunchRecordController : RestfulApiController
{
    /// <summary>
    ///     Search for launch records
    /// </summary>
    /// <param name="request">The search context</param>
    /// <returns></returns>
    [HttpGet]
    public partial IAsyncEnumerable<LaunchRecordModel> ListLaunchRecords(ListLaunchRecords.Request request);

    /// <summary>
    ///     Load details of a specific launch record
    /// </summary>
    /// <param name="id">The id of the launch record</param>
    /// <param name="request">The request context</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial Task<ActionResult<LaunchRecordModel>> GetLaunchRecord(LaunchRecordId id, GetLaunchRecord.Request request);

    /// <summary>
    ///     Create a new launch record
    /// </summary>
    /// <param name="request">The launch record details</param>
    /// <returns></returns>
    [HttpPost]
    [Created(nameof(GetLaunchRecord))]
    public partial Task<ActionResult<CreateLaunchRecord.Response>> CreateLaunchRecord(CreateLaunchRecord.Request request);

    /// <summary>
    ///     Update a given launch record
    /// </summary>
    /// <param name="id">The id of the launch record</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial Task<ActionResult> EditLaunchRecord([BindRequired] [FromRoute] LaunchRecordId id, EditLaunchRecord.Request model);

    /// <summary>
    ///     Update a given launch record
    /// </summary>
    /// <param name="id">The id of the launch record</param>
    /// <param name="model">The request details</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}")]
    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
    public partial Task<ActionResult> PatchLaunchRecord([BindRequired] [FromRoute] LaunchRecordId id, EditLaunchRecord.PatchRequest model);

    /// <summary>
    ///     Remove a launch record
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public partial Task<ActionResult> DeleteLaunchRecord(DeleteLaunchRecord.Request request);
}
