using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.Restful;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Restful.Controllers
{
    [Route("[controller]")]
    public class WeatherForecastController : RestfulApiController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(
                    index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    }
                )
               .ToArray();
        }
    }

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
        public Task<ActionResult<Guid>> CreateRocket([BindRequired, FromBody] CreateRocket.Request request) => Send(
            request,
            x => CreatedAtAction(nameof(GetRocket), new { id = x }, null)
        );

        [HttpPut("{id:guid}")]
        public Task<ActionResult<RocketModel>> UpdateRocket([BindRequired, FromRoute] Guid id, [BindRequired, FromBody] EditRocket.Model model)
            => Send(EditRocket.CreateRequest(id, model, _mapper), x => Ok(x));

        [HttpDelete("{id:guid}")]
        public Task<ActionResult<Unit>> RemoveRocket([BindRequired, FromRoute] DeleteRocket.Request request) => Send(request, x => Ok(x));
    }
}