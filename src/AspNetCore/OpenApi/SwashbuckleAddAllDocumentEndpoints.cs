using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class SwashbuckleAddAllDocumentEndpoints : IConfigureOptions<SwaggerUIOptions>
{
    private readonly IOptions<SwaggerGenOptions> _options;

    public SwashbuckleAddAllDocumentEndpoints(IOptions<SwaggerGenOptions> options)
    {
        _options = options;
    }

    public void Configure(SwaggerUIOptions options)
    {
        foreach (var item in _options.Value.SwaggerGeneratorOptions.SwaggerDocs)
        {
            options.SwaggerEndpoint($"/swagger/{item.Key}/swagger.json", item.Value.Title);
        }
    }
}
