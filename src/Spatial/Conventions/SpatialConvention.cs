using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Spatial.Conventions;

/// <summary>
///     Adds support for spatial types into STJ
/// </summary>
[PublicAPI]
[ExportConvention]
public class SpatialConvention : IServiceConvention, ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Destructure.NetTopologySuiteTypes();
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
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
