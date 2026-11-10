using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Helpers for during application startup and in middleware
/// </summary>
public static class LaunchPadHelpers
{
    /// <summary>
    /// The default response writer for health checks
    /// </summary>
    /// <remarks>
    ///     Each health check data value is written with the metadata the application's HTTP JSON options resolve for it
    ///     (<c>ConfigureHttpJsonOptions</c>, or a resolver added with <c>AddJsonTypeInfoResolver</c>), so the writer stays trim and native AOT safe.
    ///     A value no resolver knows is written as its <see cref="object.ToString" />.
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="report"></param>
    /// <returns></returns>
    public static Task DefaultResponseWriter(HttpContext context, HealthReport report)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(report);
        context.Response.ContentType = "application/json; charset=utf-8";
        var serializerOptions = context.RequestServices?.GetService<IOptions<HttpJsonOptions>>()?.Value.SerializerOptions;

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

                    WriteValue(jsonWriter, item.Value, serializerOptions);
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

    private static void WriteValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions? options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        if (value is string text)
        {
            writer.WriteStringValue(text);
            return;
        }

        if (TryGetTypeInfo(options, value.GetType()) is { } typeInfo)
        {
            JsonSerializer.Serialize(writer, value, typeInfo);
            return;
        }

        writer.WriteStringValue(value.ToString());
    }

    // The type info is resolved before anything is written, so a type no resolver supports falls back without leaving a half written value.
    private static JsonTypeInfo? TryGetTypeInfo(JsonSerializerOptions? options, Type type)
    {
        if (options is null) return null;
        try
        {
            return options.TryGetTypeInfo(type, out var typeInfo) ? typeInfo : null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
