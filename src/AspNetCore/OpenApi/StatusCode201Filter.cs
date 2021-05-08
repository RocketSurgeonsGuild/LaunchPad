using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi
{
    class StatusCode201Filter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // enhance the 201 with the response header
            if (!operation.Responses.TryGetValue("201", out var value))
                return;
            value.Headers.Add(
                "location",
                new OpenApiHeader
                {
                    // Required = true,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = "The location of the entity that was created"
                }
            );
        }
    }
}