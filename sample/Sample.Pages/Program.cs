using System.Text;
using System.Text.Json;
using Humanizer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = await builder.ConfigureRocketSurgery();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseLaunchPadRequestLogging();

app.UseRouting();

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
app.MapRazorPages();

await app.RunAsync();

static async Task WriteResponse(HttpContext context, HealthReport healthReport)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions { Indented = true };

    await using var memoryStream = new MemoryStream();
    await using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
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

            if (healthReportEntry.Value.Exception is { })
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

    await context.Response.WriteAsync(
        Encoding.UTF8.GetString(memoryStream.ToArray())
    );
}

namespace Sample.Pages
{
    public partial class Program;
}
