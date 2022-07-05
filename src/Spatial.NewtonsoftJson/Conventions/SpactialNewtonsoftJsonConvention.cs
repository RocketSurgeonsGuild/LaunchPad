using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Spatial.Conventions;

/// <summary>
///     Adds support for spatial types into Newtonsoft Json
/// </summary>
[PublicAPI]
[ExportConvention]
public class SpatialNewtonsoftJsonConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.TryAddSingleton(_ => NtsGeometryServices.Instance.CreateGeometryFactory(4326));
        services
           .AddOptions<JsonSerializerSettings>(null)
           .Configure<GeometryFactory>(
                (options, factory) => options.ConfigureGeoJsonForLaunchPad(factory)
            );
    }
}
