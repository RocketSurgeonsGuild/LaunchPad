using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using NodaTime;

using Sample.Core.Models;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     The schemas of the types the samples' OpenAPI documents would otherwise leave empty: the strongly typed ids (StronglyTypedId, which
///     LaunchPad's strong types don't describe) are UUIDs, and NodaTime's points in time are date-times, as they are written in JSON. The
///     sample tests generate their clients from these documents, and a generator can't type an empty schema.
/// </summary>
internal static class SampleOpenApiSchemas
{
    public static IServiceCollection AddSampleOpenApiSchemas(this IServiceCollection services) =>
        services.ConfigureAll<OpenApiOptions>(options => options.AddSchemaTransformer<Transformer>());

    private sealed class Transformer : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            var type = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
            if (type == typeof(RocketId) || type == typeof(LaunchRecordId))
            {
                schema.Type = JsonSchemaType.String;
                schema.Format = "uuid";
            }
            else if (type == typeof(OffsetDateTime) || type == typeof(Instant))
            {
                schema.Type = JsonSchemaType.String;
                schema.Format = "date-time";
            }

            return Task.CompletedTask;
        }
    }
}
