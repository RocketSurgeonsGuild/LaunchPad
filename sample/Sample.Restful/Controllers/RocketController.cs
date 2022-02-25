using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public partial class RocketController : RestfulApiController
{
    [HttpGet]
    public partial IAsyncEnumerable<RocketModel> ListRockets(ListRockets.Request request);

    [HttpGet("{id:guid}")]
    public partial Task<ActionResult<RocketModel>> GetRocket(GetRocket.Request request);

    [HttpPost]
    [Created(nameof(GetRocket))]
    public partial Task<ActionResult<CreateRocket.Response>> CreateRocket(CreateRocket.Request request);

    [HttpPut("{id:guid}")]
    public partial Task<ActionResult<RocketModel>> EditRocket([BindRequired] [FromRoute] Guid id, [BindRequired] [FromBody] EditRocket.Request model);

    [HttpDelete("{id:guid}")]
    public partial Task<ActionResult> RemoveRocket([BindRequired] [FromRoute] DeleteRocket.Request request);
}
