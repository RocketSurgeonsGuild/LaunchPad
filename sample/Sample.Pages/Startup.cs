using System.Text;
using System.Text.Json;
using Humanizer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Sample.Pages;

public class Startup
{
    private static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

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
                        jsonWriter, item.Value,
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

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllersWithViews();
        // services.Configure<ProblemDetailsOptions>(
        //     x =>
        //     {
        //
        //     });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
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

        app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapHealthChecks(
                    "/health", new HealthCheckOptions
                    {
                        ResponseWriter = WriteResponse,
                        ResultStatusCodes = new Dictionary<HealthStatus, int>
                        {
                            { HealthStatus.Healthy, StatusCodes.Status200OK },
                            { HealthStatus.Degraded, StatusCodes.Status500InternalServerError },
                            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable }
                        }
                    }
                );
                endpoints.MapRazorPages();
            }
        );
    }
}
