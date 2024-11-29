using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class StatusCode201Filter : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // enhance the 201 with the response header
        if (!operation.Responses.TryGetValue("201", out var value))
            return Task.CompletedTask;
        value.Headers.Add(
            "location",
            new OpenApiHeader
            {
                // Required = true,
                Schema = new OpenApiSchema { Type = "string" },
                Description = "The location of the entity that was created"
            }
        );
        return Task.CompletedTask;
    }
}
