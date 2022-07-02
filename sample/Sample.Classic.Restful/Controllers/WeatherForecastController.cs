using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Sample.Classic.Restful.Controllers;

[Route("[controller]")]
public class WeatherForecastController : RestfulApiController
{
    private static readonly string[] Summaries =
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
