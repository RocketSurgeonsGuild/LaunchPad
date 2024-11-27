using System.Reflection;
using System.Text;
using System.Text.Json;
using FluentValidation;
using Humanizer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Restful;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddHostedService<CustomHostedService>();

var app = await builder
   .LaunchWith(RocketBooster.For(Imports.Instance));
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

public partial class Program;

internal class CustomHostedServiceOptions
{
    public string? A { get; set; }

    [UsedImplicitly]
    private sealed class Validator : AbstractValidator<CustomHostedServiceOptions>
    {
        public Validator()
        {
            RuleFor(z => z.A).NotNull();
        }
    }
}

internal class CustomHostedService(IOptions<CustomHostedServiceOptions> options) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ReSharper disable once UnusedVariable
        var v = options.Value.A;
        return Task.CompletedTask;
    }
}
