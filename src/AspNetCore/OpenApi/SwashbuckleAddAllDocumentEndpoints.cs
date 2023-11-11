using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class SwashbuckleAddAllDocumentEndpoints(IOptions<SwaggerGenOptions> options) : IConfigureOptions<SwaggerUIOptions>
{
    public void Configure(SwaggerUIOptions options1)
    {
        foreach (var item in options.Value.SwaggerGeneratorOptions.SwaggerDocs)
        {
            options1.SwaggerEndpoint($"/swagger/{item.Key}/swagger.json", item.Value.Title);
        }
    }
}
