using Microsoft.AspNetCore.OpenApi;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>The OpenAPI document the HTTP test client is generated from.</summary>
internal static class FixtureOpenApi
{
    /// <summary>
    ///     OpenAPI 3.0, which NSwag reads; and nested types (<c>CreateRocket.Request</c>) named with their container, or every nested <c>Request</c>
    ///     would be one <c>Request</c> schema.
    /// </summary>
    public static IServiceCollection AddFixtureOpenApi(this IServiceCollection services) =>
        services.AddOpenApi(
            options =>
            {
                options.OpenApiVersion = OpenApi.OpenApiSpecVersion.OpenApi3_0;
                options.CreateSchemaReferenceId = type => type.Type.DeclaringType is { } outer && !type.Type.IsGenericType
                    ? outer.Name + type.Type.Name
                    : OpenApiOptions.CreateDefaultSchemaReferenceId(type);
            }
        );
}
