using System.Text.Json;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Helpers for during application startup and in middleware
/// </summary>
public static class LaunchPadHelpers
{
    /// <summary>
    /// The default response writer for health checks
    /// </summary>
    /// <param name="context"></param>
    /// <param name="report"></param>
    /// <returns></returns>
    public static Task DefaultResponseWriter(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true, };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", report.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in report.Entries)
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
            System.Text.Encoding.UTF8.GetString(memoryStream.ToArray())
        );

    }
}
