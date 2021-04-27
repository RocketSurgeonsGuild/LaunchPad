using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: Convention(typeof(AspNetCoreSpatialConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    public class AspNetCoreSpatialConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
            => services.Configure<SwaggerGenOptions>(o => o.ConfigureForNetTopologySuite());
    }
}