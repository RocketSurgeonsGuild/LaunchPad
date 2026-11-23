using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using NodaTime;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     The schemas of the types the samples' OpenAPI documents would otherwise leave empty: NodaTime's points in time are date-times, as they
///     are written in JSON. The sample tests generate their clients from these documents, and a generator can't type an empty schema. The
///     strong IDs (RocketId, LaunchRecordId) are domain primitives, which the OpenAPI joint of LaunchPad.Primitives describes as UUIDs.
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
            if (type == typeof(OffsetDateTime) || type == typeof(Instant))
            {
                schema.Type = JsonSchemaType.String;
                schema.Format = "date-time";
            }

            return Task.CompletedTask;
        }
    }
}
