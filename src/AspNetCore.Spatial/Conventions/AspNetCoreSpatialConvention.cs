using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     Convention to register spatial types
/// </summary>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Application)]
public class AspNetCoreSpatialConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
//        services.Configure<SwaggerGenOptions>(o => o.ConfigureForNetTopologySuite());
    }
}
