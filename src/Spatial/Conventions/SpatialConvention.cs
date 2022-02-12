using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Spatial.Conventions;

[assembly: Convention(typeof(SpatialConvention))]

namespace Rocket.Surgery.LaunchPad.Spatial.Conventions;

/// <summary>
///     Adds support for spatial types into STJ
/// </summary>
public class SpatialConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.TryAddSingleton(_ => NtsGeometryServices.Instance.CreateGeometryFactory(4326));
        services
           .AddOptions<JsonSerializerOptions>(null)
           .Configure<GeometryFactory>(
                (options, factory) => options.ConfigureNetTopologySuiteForLaunchPad(factory)
            );
    }
}
