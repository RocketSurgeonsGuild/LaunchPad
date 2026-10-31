using System.Text.Json;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Spatial.Joints;

/// <summary>
///     Adds support for spatial types into System.Text.Json and Serilog
/// </summary>
/// <remarks>
///     A service Joint that registers the geometry services and configures the System.Text.Json options,
///     and a Serilog Joint that destructures the NetTopologySuite types.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class SpatialJoint : IServiceJoint, ISerilogJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Destructure.NetTopologySuiteTypes();
    }

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
