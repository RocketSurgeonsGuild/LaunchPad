using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public partial class LaunchRecordController : RestfulApiController
{
    [HttpGet]
    public partial Task<ActionResult<IEnumerable<LaunchRecordModel>>> ListLaunchRecords(ListLaunchRecords.Request request);

    [HttpGet("{id:guid}")]
    public partial Task<ActionResult<LaunchRecordModel>> GetLaunchRecord(GetLaunchRecord.Request request);

    [HttpPost]
    [Created(nameof(GetLaunchRecord))]
    public partial Task<ActionResult<CreateLaunchRecord.Response>> CreateLaunchRecord(CreateLaunchRecord.Request request);

    [HttpPut("{id:guid}")]
    public partial Task<ActionResult> EditLaunchRecord([BindRequired] [FromRoute] Guid id, EditLaunchRecord.Request model);

    [HttpDelete("{id:guid}")]
    public partial Task<ActionResult> DeleteLaunchRecord(DeleteLaunchRecord.Request request);
}
