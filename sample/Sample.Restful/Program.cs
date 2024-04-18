using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Humanizer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Restful;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder =  WebApplication
                   .CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddHostedService<CustomHostedService>();
builder.Services
       .Configure<SwaggerGenOptions>(
            c => c.SwaggerDoc(
                "v1",
                new()
                {
                    Version = typeof(Program).GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                     ?? typeof(Program).GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.1.0",
                    Title = "Test Application",
                }
            )
        );

var app = await builder
         .LaunchWith(RocketBooster.For(Imports.Instance), b => b.Set(AssemblyLoadContext.Default));
app.UseProblemDetails();
app.UseHttpsRedirection();

// Should this move into an extension method?
app.UseSerilogRequestLogging(
    x =>
    {
        x.GetLevel = LaunchPadLogHelpers.DefaultGetLevel;
        x.EnrichDiagnosticContext = LaunchPadLogHelpers.DefaultEnrichDiagnosticContext;
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
        ResponseWriter = WriteResponse,
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK },
            { HealthStatus.Degraded, StatusCodes.Status500InternalServerError },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
        },
    }
);

app.MapControllers();

// Should this move into an extension method?
app.MapSwagger();

app.Run();

static Task WriteResponse(HttpContext context, HealthReport healthReport)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions { Indented = true, };

    using var memoryStream = new MemoryStream();
    using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", healthReport.Status.ToString());
        jsonWriter.WriteStartObject("results");

        foreach (var healthReportEntry in healthReport.Entries)
        {
            jsonWriter.WriteStartObject(healthReportEntry.Key);
            jsonWriter.WriteString(
                "status",
                healthReportEntry.Value.Status.ToString()
            );
            jsonWriter.WriteString(
                "duration",
                healthReportEntry.Value.Duration.Humanize()
            );
            jsonWriter.WriteString(
                "description",
                healthReportEntry.Value.Description
            );

            jsonWriter.WriteStartObject("data");
            foreach (var item in healthReportEntry.Value.Data)
            {
                jsonWriter.WritePropertyName(item.Key);

                JsonSerializer.Serialize(
                    jsonWriter,
                    item.Value,
                    item.Value.GetType()
                );
            }

            jsonWriter.WriteEndObject();

            if (healthReportEntry.Value.Tags.Any())
            {
                jsonWriter.WriteStartArray("tags");
                foreach (var item in healthReportEntry.Value.Tags)
                {
                    jsonWriter.WriteStringValue(item);
                }

                jsonWriter.WriteEndArray();
            }

            if (healthReportEntry.Value.Exception != null)
            {
                var ex = healthReportEntry.Value.Exception;
                jsonWriter.WriteStartObject("exception");
                jsonWriter.WriteString("message", ex.Message);
                jsonWriter.WriteString("stacktrace", ex.StackTrace);
                jsonWriter.WriteString("inner", ex.InnerException?.ToString());
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }

    return context.Response.WriteAsync(
        Encoding.UTF8.GetString(memoryStream.ToArray())
    );
}

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
