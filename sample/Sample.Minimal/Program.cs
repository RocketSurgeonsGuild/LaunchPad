using Microsoft.Extensions.Diagnostics.HealthChecks;

using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Minimal;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();

// LaunchPad.Hosting still registers the Serilog request logging services with a convention that Dovetail does not run,
// until its Serilog joint replaces it (RocketSurgeonsGuild/LaunchPad#3062)
builder.Services.AddSerilog(logger => logger.WriteTo.Console());

var app = await builder.ConfigureDovetail();
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
