using System.Text.Json;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Helpers for during application startup and in middleware
/// </summary>
public static class LaunchPadHelpers
{
    /// <summary>
    ///     Setup the <see cref="IDiagnosticContext" /> with default values from the request
    /// </summary>
    /// <param name="diagnosticContext"></param>
    /// <param name="httpContext"></param>
    public static void DefaultEnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        // Only set it if available. You're not sending sensitive data in a querystring right?!
        if (request.QueryString.HasValue) diagnosticContext.Set("QueryString", request.QueryString.Value);

        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is { }) // endpoint != null
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
    }

    /// <summary>
    ///     Gets the default logging level based on the response status code
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="_"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static LogEventLevel DefaultGetLevel(HttpContext ctx, double _, Exception? ex)
    {
        return ex is { }
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                    ? LogEventLevel.Verbose  // Was a health check, use Verbose
                    : LogEventLevel.Information;
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is { }) // same as !(endpoint is null)
            return string.Equals(endpoint.DisplayName, "Health checks", StringComparison.Ordinal);

        // No endpoint, so not a health check endpoint
        return false;
    }

    /// <summary>
    /// The default response writer for health checks
    /// </summary>
    /// <param name="context"></param>
    /// <param name="report"></param>
    /// <returns></returns>
    public static Task DefaultResponseWriter (HttpContext context, HealthReport report)
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
