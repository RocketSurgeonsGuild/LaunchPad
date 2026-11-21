using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Writes a <see cref="HealthReport" /> as JSON in the shape the HealthChecks.UI dashboard reads (<c>UIResponseWriter</c>).
/// </summary>
/// <remarks>
///     <para>
///         The report is written with a <see cref="Utf8JsonWriter" />, never through reflection, so the writer is trim and native AOT safe
///         (ADR 0007). Each health check data value is written with the metadata the application's HTTP JSON options resolve for it
///         (<c>ConfigureHttpJsonOptions</c>, or a resolver added with <c>AddJsonTypeInfoResolver</c>); a value no resolver knows is written as its
///         <see cref="object.ToString" />.
///     </para>
///     <para>
///         The <c>exception</c> of an entry is the whole <see cref="Exception.ToString" />: its type, message, stack trace and inner exceptions.
///         Serve the report on a port of its own (<see cref="LaunchPadHealthCheckOptions.Port" />) when it must not be public.
///     </para>
/// </remarks>
/// <example>
///     <code language="json">
///     {
///       "status": "Healthy",
///       "totalDuration": "00:00:00.0012345",
///       "entries": {
///         "db": { "data": {}, "description": "fine", "duration": "00:00:00.0010000", "status": "Healthy", "tags": ["ready"] }
///       }
///     }
///     </code>
/// </example>
[PublicAPI]
public static class LaunchPadHealthCheckResponseWriter
{
    /// <summary>The content type the report is written with.</summary>
    public const string ContentType = "application/json; charset=utf-8";

    /// <summary>
    ///     Writes the report to the response. Use it as a <c>HealthCheckOptions.ResponseWriter</c>.
    /// </summary>
    /// <param name="context">The context of the request.</param>
    /// <param name="report">The report to write.</param>
    public static async Task WriteAsync(HttpContext context, HealthReport report)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(report);

        context.Response.ContentType = ContentType;
        var serializerOptions = context.RequestServices?.GetService<IOptions<HttpJsonOptions>>()?.Value.SerializerOptions;

        await using var writer = new Utf8JsonWriter(context.Response.Body);
        Write(writer, report, serializerOptions);
        await writer.FlushAsync(context.RequestAborted);
    }

    /// <summary>
    ///     Writes the report as a JSON object.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="report">The report to write.</param>
    /// <param name="serializerOptions">The options whose resolvers write the health check data values, if any.</param>
    public static void Write(Utf8JsonWriter writer, HealthReport report, JsonSerializerOptions? serializerOptions = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(report);

        writer.WriteStartObject();
        writer.WriteString("status", report.Status.ToString());
        writer.WriteString("totalDuration", report.TotalDuration.ToString());
        writer.WriteStartObject("entries");
        foreach (var (name, entry) in report.Entries)
        {
            writer.WriteStartObject(name);

            writer.WriteStartObject("data");
            foreach (var (key, value) in entry.Data)
            {
                writer.WritePropertyName(key);
                WriteValue(writer, value, serializerOptions);
            }

            writer.WriteEndObject();

            // As HealthChecks.UI does, an entry with no description of its own is described by its exception's message.
            if (( entry.Description ?? entry.Exception?.Message ) is { } description) writer.WriteString("description", description);
            writer.WriteString("duration", entry.Duration.ToString());
            if (entry.Exception is { } exception) writer.WriteString("exception", exception.ToString());
            writer.WriteString("status", entry.Status.ToString());

            writer.WriteStartArray("tags");
            foreach (var tag in entry.Tags)
            {
                writer.WriteStringValue(tag);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private static void WriteValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions? options)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                return;
            case string text:
                writer.WriteStringValue(text);
                return;
            default:
                break;
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
