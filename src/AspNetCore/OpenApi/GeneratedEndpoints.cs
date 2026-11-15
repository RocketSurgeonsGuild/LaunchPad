using System.CodeDom.Compiler;
using Microsoft.AspNetCore.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

/// <summary>
///     Tells the endpoints LaunchPad's API generators built from hand-written ones. A generated endpoint carries
///     <see cref="GeneratedCodeAttribute" /> metadata with the tool <c>Rocket.Surgery.LaunchPad.Api</c> and its own OpenAPI operation transformer, so
///     the filters here leave it alone and nothing is applied twice.
/// </summary>
internal static class GeneratedEndpoints
{
    private const string Tool = "Rocket.Surgery.LaunchPad.Api";

    public static bool IsGenerated(this OpenApiOperationTransformerContext context)
    {
        foreach (var metadata in context.Description.ActionDescriptor.EndpointMetadata)
        {
            if (metadata is GeneratedCodeAttribute { Tool: Tool }) return true;
        }

        return false;
    }
}
