using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.Restful;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Restful.Controllers
{
    [Route("[controller]")]
    public class LaunchRecordController : RestfulApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<LaunchRecordModel>>> ListLaunchRecords() => Send(new ListLaunchRecords.Request(), x => Ok(x));

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<LaunchRecordModel>> GetLaunchRecord([BindRequired, FromRoute] GetLaunchRecord.Request request) => Send(request, x => Ok(x));

        [HttpPost]
        public Task<ActionResult<CreateLaunchRecord.Response>> CreateLaunchRecord([BindRequired, FromBody] CreateLaunchRecord.Request request) => Send(
            request,
            x => CreatedAtAction(nameof(GetLaunchRecord), new { id = x }, x)
        );

        [HttpPut("{id:guid}")]
        public Task<ActionResult> UpdateLaunchRecord([BindRequired, FromRoute] Guid id, [BindRequired, FromBody] EditLaunchRecord.Request model)
            => Send(new EditLaunchRecord.Request() { Id = id }.With(model), NoContent);

        [HttpDelete("{id:guid}")]
        public Task<ActionResult> RemoveLaunchRecord([BindRequired, FromRoute] DeleteLaunchRecord.Request request) => Send(request);
    }
}