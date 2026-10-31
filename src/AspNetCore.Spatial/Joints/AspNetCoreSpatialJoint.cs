using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Joint to register spatial types
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class AspNetCoreSpatialJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        //        services.Configure<SwaggerGenOptions>(o => o.ConfigureForNetTopologySuite());
    }
}
