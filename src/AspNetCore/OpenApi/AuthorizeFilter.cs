using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class AuthorizeFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // enhance the 401/403 with the media response
        if (operation.Responses.TryGetValue("401", out var value))
            value.Headers.Add(
                "www-authenticate",
                new OpenApiHeader
                {
                    // Required = true,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = "The error details"
                }
            );

        if (operation.Responses.TryGetValue("403", out value))
            value.Headers.Add(
                "www-authenticate",
                new OpenApiHeader
                {
                    // Required = true,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = "The error details"
                }
            );
    }
}
