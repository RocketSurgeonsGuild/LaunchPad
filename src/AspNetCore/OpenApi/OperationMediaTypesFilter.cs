using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

    internal class OperationMediaTypesFilter : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var contentCollections =
            operation
               .Responses.Values.Select(x => x.Content ?? new Dictionary<string, OpenApiMediaType>())
               .Concat([operation.RequestBody?.Content ?? new Dictionary<string, OpenApiMediaType>()])
               .Where(x => x.ContainsKey("text/plain"))
               .ToArray();
        var patchCollections =
            operation
               .Responses.Values.Select(x => x.Content ?? new Dictionary<string, OpenApiMediaType>())
               .Concat([operation.RequestBody?.Content ?? new Dictionary<string, OpenApiMediaType>()])
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

        return Task.CompletedTask;
    }
}
