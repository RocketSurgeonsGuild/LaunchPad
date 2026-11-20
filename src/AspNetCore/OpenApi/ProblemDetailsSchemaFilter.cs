using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

/// <summary>
///     Describes <see cref="ProblemDetails" /> as it goes over the wire: its extensions are written as properties of the problem itself, and
///     the validation errors are named <c>errors</c>.
/// </summary>
internal class ProblemDetailsSchemaFilter : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (!typeof(ProblemDetails).IsAssignableFrom(context.JsonTypeInfo.Type)) return Task.CompletedTask;
        schema.AdditionalPropertiesAllowed = true;
        if (schema.Properties is not { } properties) return Task.CompletedTask;
        properties.Remove(nameof(ProblemDetails.Extensions));
        properties.Remove("extensions");
        if (properties.Remove("validationErrors", out var validationErrors))
        {
            properties["errors"] = validationErrors;
        }

        return Task.CompletedTask;
    }
}
