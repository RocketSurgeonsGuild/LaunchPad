using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi
{
    class OperationMediaTypesFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var contentCollections =
                operation.Responses.Values.Select(x => x.Content ?? new Dictionary<string, OpenApiMediaType>())
                   .Concat(new[] { operation.RequestBody?.Content ?? new Dictionary<string, OpenApiMediaType>() })
                   .Where(x => x.ContainsKey("text/plain"))
                   .ToArray();
            var patchCollections =
                operation.Responses.Values.Select(x => x.Content ?? new Dictionary<string, OpenApiMediaType>())
                   .Concat(new[] { operation.RequestBody?.Content ?? new Dictionary<string, OpenApiMediaType>() })
                   .Where(x => x.Keys.Any(z => z.Contains("patch", StringComparison.OrdinalIgnoreCase)))
                   .ToArray();

            foreach (var item in contentCollections)
            {
                item.Remove("text/plain");
                // item.Add("text/plain", plain);
            }

            foreach (var item in patchCollections)
            {
                foreach (var p in item.Where(z => z.Key.Contains("patch", StringComparison.OrdinalIgnoreCase)).ToArray())
                {
                    item.Remove(p.Key);
                }
            }
        }
    }
}