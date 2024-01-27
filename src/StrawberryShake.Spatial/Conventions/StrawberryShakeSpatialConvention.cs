using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Conventions;

/// <summary>
///     Adds support for spatial types into STJ
/// </summary>
[PublicAPI]
[ExportConvention]
public class StrawberryShakeSpatialConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<ISerializer, GeometrySerializer>();
//        services.AddSerializer<CoordinatesSerializer>();
    }
}
