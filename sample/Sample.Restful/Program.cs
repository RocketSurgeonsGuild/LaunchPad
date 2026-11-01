using Microsoft.Extensions.Diagnostics.HealthChecks;

using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Restful;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();

var app = await builder.ConfigureDovetail();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapOpenApi();
app
   .UseSwaggerUI()
   .UseReDoc();

app.UseAuthorization();
app.MapHealthChecks(
    "/health",
    new()
    {
        ResponseWriter = LaunchPadHelpers.DefaultResponseWriter,
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK },
            { HealthStatus.Degraded, StatusCodes.Status500InternalServerError },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
        },
    }
);

app.MapControllers();

app.Run();

namespace Sample.Restful
{
    public partial class Program;
}
