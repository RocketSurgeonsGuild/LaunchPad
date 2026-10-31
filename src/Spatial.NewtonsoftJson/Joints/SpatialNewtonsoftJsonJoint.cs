using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Rocket.Surgery.LaunchPad.Spatial.Joints;

/// <summary>
///     Adds support for spatial types into Newtonsoft Json
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class SpatialNewtonsoftJsonJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.TryAddSingleton(_ => NtsGeometryServices.Instance.CreateGeometryFactory(4326));
        services
           .AddOptions<JsonSerializerSettings>(null)
           .Configure<GeometryFactory>(
                (options, factory) => options.ConfigureGeoJsonForLaunchPad(factory)
            );
    }
}
