using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class ProblemDetailsSchemaFilter : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (true) return Task.CompletedTask;
        if (!typeof(ProblemDetails).IsAssignableFrom(context.JsonTypeInfo.Type)) return Task.CompletedTask;
        schema.AdditionalPropertiesAllowed = true;
        schema.Properties.Remove(nameof(ProblemDetails.Extensions));
        schema.Properties.Remove("extensions");
        if (schema.Properties.TryGetValue("validationErrors", out var v))
        {
            schema.Properties["errors"] = v;
            schema.Properties.Remove("validationErrors");
        }
        return Task.CompletedTask;
    }
}
