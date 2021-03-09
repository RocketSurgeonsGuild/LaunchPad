using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi
{
    class ProblemDetailsSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (typeof(ProblemDetails).IsAssignableFrom(context.Type))
            {
                schema.AdditionalPropertiesAllowed = true;
                schema.Properties.Remove(nameof(ProblemDetails.Extensions));
                schema.Properties.Remove("extensions");
                if (schema.Properties.TryGetValue("validationErrors", out var v))
                {
                    schema.Properties["errors"] = v;
                    schema.Properties.Remove("validationErrors");
                }
            }
        }
    }
}