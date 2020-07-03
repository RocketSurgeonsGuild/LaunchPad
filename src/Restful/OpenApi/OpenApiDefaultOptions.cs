using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.ReDoc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Rocket.Surgery.LaunchPad.Restful.OpenApi
{
    class OpenApiDefaultOptions : IConfigureOptions<SwaggerUIOptions>,
                                  IConfigureOptions<SwaggerGenOptions>,
                                  IConfigureOptions<SwaggerOptions>,
                                  IConfigureOptions<ReDocOptions>
    {
        void IConfigureOptions<SwaggerUIOptions>.Configure(SwaggerUIOptions options) { }

        void IConfigureOptions<SwaggerGenOptions>.Configure(SwaggerGenOptions options) { }

        void IConfigureOptions<SwaggerOptions>.Configure(SwaggerOptions options) { }

        void IConfigureOptions<ReDocOptions>.Configure(ReDocOptions options) { }
    }
}