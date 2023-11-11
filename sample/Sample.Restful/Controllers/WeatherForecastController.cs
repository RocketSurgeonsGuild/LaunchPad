using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Sample.Restful.Controllers;

[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : RestfulApiController
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

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
