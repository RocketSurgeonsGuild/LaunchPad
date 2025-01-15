using Microsoft.Extensions.Diagnostics.HealthChecks;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Minimal;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();

var app = await builder.ConfigureRocketSurgery();
app.UseExceptionHandler();
app.UseHttpsRedirection();

// Should this move into an extension method?
app.UseSerilogRequestLogging(
    x =>
    {
        x.GetLevel = LaunchPadHelpers.DefaultGetLevel;
        x.EnrichDiagnosticContext = LaunchPadHelpers.DefaultEnrichDiagnosticContext;
    }
);

app.UseRouting();
app.MapOpenApi();

app
   .UseSwaggerUI()
   .UseReDoc();
app.UseLaunchRecords();
app.UseRockets();

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

app.Run();

namespace Sample.Minimal
{
    public partial class Program;
}
