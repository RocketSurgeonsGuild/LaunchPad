using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Rocket.Surgery.LaunchPad.Spatial.Joints;
using Rocket.Surgery.LaunchPad.Spatial.Rivets;

namespace Extensions.Tests;

public class SpatialNewtonsoftJsonJointTests
{
    [Test]
    public async Task Registers_The_Geometry_Factory()
    {
        var provider = await Register(new ServiceCollection());

        provider.GetRequiredService<GeometryFactory>().SRID.ShouldBe(4326);
    }

    [Test]
    public async Task Keeps_A_Geometry_Factory_That_Is_Already_Registered()
    {
        var factory = new GeometryFactory(new PrecisionModel(), 3857);

        var provider = await Register(new ServiceCollection().AddSingleton(factory));

        provider.GetRequiredService<GeometryFactory>().ShouldBeSameAs(factory);
    }

    [Test]
    public async Task Configures_The_Newtonsoft_Json_Settings_For_GeoJson()
    {
        var provider = await Register(new ServiceCollection());
        var settings = provider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value;

        JsonConvert.DeserializeObject<Point>("""{"type": "Point","coordinates": [30, 10]}""", settings).ShouldBe(new Point(30, 10));
        JsonConvert.SerializeObject(new Point(30, 10), settings).ShouldBe("""{"type":"Point","coordinates":[30.0,10.0]}""");
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NewtonsoftJsonExports.Joints().Single(x => x.Joint is SpatialNewtonsoftJsonJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        typeof(SpatialNewtonsoftJsonJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private static async Task<IServiceProvider> Register(IServiceCollection services)
    {
        new SpatialNewtonsoftJsonJoint().Register(await JointContext.Create(), services);
        return services.BuildServiceProvider();
    }
}
