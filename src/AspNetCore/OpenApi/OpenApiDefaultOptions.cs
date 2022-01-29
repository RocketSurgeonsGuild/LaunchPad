using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.ReDoc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class OpenApiDefaultOptions : IConfigureOptions<SwaggerUIOptions>,
                                       IConfigureOptions<SwaggerGenOptions>,
                                       IConfigureOptions<SwaggerOptions>,
                                       IConfigureOptions<ReDocOptions>
{
    void IConfigureOptions<ReDocOptions>.Configure(ReDocOptions options)
    {
    }

    void IConfigureOptions<SwaggerGenOptions>.Configure(SwaggerGenOptions options)
    {
    }

    void IConfigureOptions<SwaggerOptions>.Configure(SwaggerOptions options)
    {
    }

    void IConfigureOptions<SwaggerUIOptions>.Configure(SwaggerUIOptions options)
    {
        options.ConfigObject.DeepLinking = true;
        options.ConfigObject.ShowExtensions = true;
        options.ConfigObject.ShowCommonExtensions = true;
        options.ConfigObject.Filter = string.Empty;
        options.ConfigObject.DisplayRequestDuration = true;
        options.ConfigObject.DisplayOperationId = true;
    }
}
