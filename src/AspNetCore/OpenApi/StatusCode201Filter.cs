using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class StatusCode201Filter : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // enhance the 201 with the response header
        if (operation.Responses?.TryGetValue("201", out var value) is not true || value is not OpenApiResponse response)
            return Task.CompletedTask;
        // Microsoft.OpenApi leaves the headers null until one is added
        response.Headers ??= new Dictionary<string, IOpenApiHeader>(StringComparer.Ordinal);
        response.Headers["location"] = new OpenApiHeader
        {
            // Required = true,
            Schema = new OpenApiSchema { Type = JsonSchemaType.String },
            Description = "The location of the entity that was created",
        };
        return Task.CompletedTask;
    }
}
