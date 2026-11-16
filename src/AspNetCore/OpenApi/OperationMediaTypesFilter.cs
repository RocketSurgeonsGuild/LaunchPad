using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class OperationMediaTypesFilter : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.IsGenerated()) return Task.CompletedTask;
        foreach (var response in operation.Responses?.Values ?? Enumerable.Empty<IOpenApiResponse>())
        {
            RemoveMediaTypes(response.Content);
        }

        RemoveMediaTypes(operation.RequestBody?.Content);
        return Task.CompletedTask;
    }

    private static void RemoveMediaTypes<TMediaType>(IDictionary<string, TMediaType>? content)
    {
        if (content is null)
        {
            return;
        }

        foreach (var mediaType in content.Keys.Where(IsRemovedMediaType).ToArray())
        {
            content.Remove(mediaType);
        }
    }

    private static bool IsRemovedMediaType(string mediaType) =>
        mediaType == "text/plain" || mediaType.Contains("patch", StringComparison.OrdinalIgnoreCase);
}
