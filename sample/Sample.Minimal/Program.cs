using Microsoft.Extensions.Diagnostics.HealthChecks;

using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddHostedService<CustomHostedService>();

var app = await builder
   .ConfigureRocketSurgery();
app.UseExceptionHandler();
app.UseHttpsRedirection();

// Should this move into an extension method?
app.UseSerilogRequestLogging(
    x =>
    {
        x.GetLevel = (f, f2, f3) => LaunchPadHelpers.DefaultGetLevel(f, f2, f3);
        x.EnrichDiagnosticContext = (f, f2) => LaunchPadHelpers.DefaultEnrichDiagnosticContext(f, f2);
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
        ResponseWriter = (f, f2) => LaunchPadHelpers.DefaultResponseWriter(f, f2),
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK },
            { HealthStatus.Degraded, StatusCodes.Status500InternalServerError },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
        },
    }
);

app.Run();
