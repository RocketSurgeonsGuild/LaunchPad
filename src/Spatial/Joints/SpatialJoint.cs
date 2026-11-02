using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Rocket.Surgery.LaunchPad.Spatial.Joints;

/// <summary>
///     Adds support for spatial types into System.Text.Json
/// </summary>
/// <remarks>
///     A service Joint that registers the geometry services and configures the System.Text.Json options.
///     The NetTopologySuite types are destructured in Serilog by the <see cref="SpatialSerilogJoint" /> of this package.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class SpatialJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.TryAddSingleton(NtsGeometryServices.Instance);
        services.TryAddSingleton(provider => provider.GetRequiredService<NtsGeometryServices>().CreateGeometryFactory(4326));
        services
           .AddOptions<JsonSerializerOptions>(null)
           .Configure<GeometryFactory>(
                (options, factory) => options.ConfigureGeoJsonForLaunchPad(factory)
            );
    }
}
