using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class OperationMediaTypesFilter : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // The content dictionary's value type differs between Microsoft.OpenApi 2.x (net10.0, OpenApiMediaType) and 3.x (net11.0,
        // IOpenApiMediaType), so skip missing content instead of substituting an empty dictionary of one particular type.
        var contentCollections =
            ( operation.Responses?.Values ?? Enumerable.Empty<IOpenApiResponse>() )
               .Select(x => x.Content)
               .Append(operation.RequestBody?.Content)
               .Where(x => x is not null && x.ContainsKey("text/plain"))
               .Select(x => x!)
               .ToArray();
        var patchCollections =
            ( operation.Responses?.Values ?? Enumerable.Empty<IOpenApiResponse>() )
               .Select(x => x.Content)
               .Append(operation.RequestBody?.Content)
               .Where(x => x is not null && x.Keys.Any(z => z.Contains("patch", StringComparison.OrdinalIgnoreCase)))
               .Select(x => x!)
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
