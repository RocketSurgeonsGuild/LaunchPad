﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.Restful;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Restful.Controllers
{
    [Route("[controller]")]
    public class RocketController : RestfulApiController
    {
        private readonly IMapper _mapper;
        public RocketController(IMapper mapper) => _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<RocketModel>>> ListRockets() => Send(new ListRockets.Request(), x => Ok(x));

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<RocketModel>> GetRocket([BindRequired, FromRoute] GetRocket.Request request) => Send(request, x => Ok(x));

        [HttpPost]
        public Task<ActionResult<CreateRocket.Response>> CreateRocket([BindRequired, FromBody] CreateRocket.Request request) => Send(
            request,
            x => CreatedAtAction(nameof(GetRocket), new { id = x.Id }, null)
        );

        [HttpPut("{id:guid}")]
        public Task<ActionResult> UpdateRocket([BindRequired, FromRoute] Guid id, [BindRequired, FromBody] EditRocket.Model model)
            => Send(EditRocket.CreateRequest(id, model, _mapper), NoContent);

        [HttpDelete("{id:guid}")]
        public Task<ActionResult> RemoveRocket([BindRequired, FromRoute] DeleteRocket.Request request) => Send(request);
    }

    [Route("[controller]")]
    public class LaunchRecordController : RestfulApiController
    {
        private readonly IMapper _mapper;
        public LaunchRecordController(IMapper mapper) => _mapper = mapper;

        // [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public Task<ActionResult<IEnumerable<RocketModel>>> ListRockets() => Send(new ListRockets.Request(), x => Ok(x));
        //
        // [HttpGet("{id:guid}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public Task<ActionResult<RocketModel>> GetRocket([BindRequired, FromRoute] GetRocket.Request request) => Send(request, x => Ok(x));
        //
        // [HttpPost]
        // public Task<ActionResult<CreateRocket.Response>> CreateRocket([BindRequired, FromBody] CreateRocket.Request request) => Send(
        //     request,
        //     x => CreatedAtAction(nameof(GetRocket), new { id = x }, null)
        // );
        //
        // [HttpPut("{id:guid}")]
        // public Task<ActionResult> UpdateRocket([BindRequired, FromRoute] Guid id, [BindRequired, FromBody] EditRocket.Model model)
        //     => Send(EditRocket.CreateRequest(id, model, _mapper), NoContent);
        //
        // [HttpDelete("{id:guid}")]
        // public Task<ActionResult> RemoveRocket([BindRequired, FromRoute] DeleteRocket.Request request) => Send(request);
    }
}