using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class AuthorizeFilter : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // enhance the 401/403 with the media response
        AddAuthenticateHeader(operation, "401");
        AddAuthenticateHeader(operation, "403");
        return Task.CompletedTask;
    }

    private static void AddAuthenticateHeader(OpenApiOperation operation, string statusCode)
    {
        if (operation.Responses?.TryGetValue(statusCode, out var value) is not true || value is not OpenApiResponse response) return;
        // Microsoft.OpenApi leaves the headers null until one is added
        response.Headers ??= new Dictionary<string, IOpenApiHeader>(StringComparer.Ordinal);
        response.Headers["www-authenticate"] = new OpenApiHeader
        {
            // Required = true,
            Schema = new OpenApiSchema { Type = JsonSchemaType.String, },
            Description = "The error details",
        };
    }
}
