using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class AuthorizeFilter : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // enhance the 401/403 with the media response
        if (operation.Responses.TryGetValue("401", out var value))
            value.Headers.Add(
                "www-authenticate",
                new OpenApiHeader
                {
                    // Required = true,
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String, },
                    Description = "The error details",
                }
            );

        if (operation.Responses.TryGetValue("403", out value))
            value.Headers.Add(
                "www-authenticate",
                new OpenApiHeader()
                {
                    // Required = true,
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String, },
                    Description = "The error details",
                }
            );

        return Task.CompletedTask;
    }
}
