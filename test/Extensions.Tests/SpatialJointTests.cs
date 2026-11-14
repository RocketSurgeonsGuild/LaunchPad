using System.Reflection;
using System.Text.Json;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Rocket.Surgery.LaunchPad.Spatial.Joints;
using Rocket.Surgery.LaunchPad.Spatial.Rivets;

namespace Extensions.Tests;

public class SpatialJointTests
{
    [Test]
    public async Task Registers_The_Geometry_Services()
    {
        var provider = await Register(new ServiceCollection());

        await provider.GetRequiredService<NtsGeometryServices>().Should().BeSameReferenceAs(NtsGeometryServices.Instance);
        await provider.GetRequiredService<GeometryFactory>().SRID.Should().BeEqualTo(4326);
    }

    [Test]
    public async Task Keeps_Geometry_Services_That_Are_Already_Registered()
    {
        var geometryServices = new NtsGeometryServices();
        var factory = geometryServices.CreateGeometryFactory(3857);

        var provider = await Register(new ServiceCollection().AddSingleton(geometryServices).AddSingleton(factory));

        await provider.GetRequiredService<NtsGeometryServices>().Should().BeSameReferenceAs(geometryServices);
        await provider.GetRequiredService<GeometryFactory>().Should().BeSameReferenceAs(factory);
    }

    [Test]
    public async Task Configures_The_System_Text_Json_Options_For_GeoJson()
    {
        var provider = await Register(new ServiceCollection());
        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        await JsonSerializer.Deserialize<Point>("""{"type": "Point","coordinates": [30, 10]}""", options).Should().BeEqualTo(new Point(30, 10));
        await JsonSerializer.Serialize<Geometry>(new Point(30, 10), options).Should().BeEqualTo("""{"type":"Point","coordinates":[30,10]}""");
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is SpatialJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        await typeof(SpatialJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }

    private static async Task<IServiceProvider> Register(IServiceCollection services)
    {
        new SpatialJoint().Register(await JointContext.Create(), services);
        return services.BuildServiceProvider();
    }
}
