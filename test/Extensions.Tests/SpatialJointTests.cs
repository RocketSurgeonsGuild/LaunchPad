using System.Reflection;
using System.Text.Json;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Rocket.Surgery.LaunchPad.Spatial;
using Rocket.Surgery.LaunchPad.Spatial.Joints;
using Rocket.Surgery.LaunchPad.Spatial.Rivets;

namespace Extensions.Tests;

public class SpatialJointTests
{
    [Test]
    public async Task Registers_The_Geometry_Services()
    {
        var provider = await Register(new ServiceCollection());

        provider.GetRequiredService<NtsGeometryServices>().ShouldBeSameAs(NtsGeometryServices.Instance);
        provider.GetRequiredService<GeometryFactory>().SRID.ShouldBe(4326);
    }

    [Test]
    public async Task Keeps_Geometry_Services_That_Are_Already_Registered()
    {
        var geometryServices = new NtsGeometryServices();
        var factory = geometryServices.CreateGeometryFactory(3857);

        var provider = await Register(new ServiceCollection().AddSingleton(geometryServices).AddSingleton(factory));

        provider.GetRequiredService<NtsGeometryServices>().ShouldBeSameAs(geometryServices);
        provider.GetRequiredService<GeometryFactory>().ShouldBeSameAs(factory);
    }

    [Test]
    public async Task Configures_The_System_Text_Json_Options_For_GeoJson()
    {
        var provider = await Register(new ServiceCollection());
        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        JsonSerializer.Deserialize<Point>("""{"type": "Point","coordinates": [30, 10]}""", options).ShouldBe(new Point(30, 10));
        JsonSerializer.Serialize<Geometry>(new Point(30, 10), options).ShouldBe("""{"type":"Point","coordinates":[30,10]}""");
    }

    [Test]
    public async Task Destructures_The_NetTopologySuite_Types_As_The_Serilog_Extension_Does()
    {
        var context = await JointContext.Create();
        var geometry = new Point(30, 10);

        var withJoint = JointContext.RenderDestructured(configuration => new SpatialJoint().Register(context, new ServiceCollection(), configuration), geometry);
        var withExtension = JointContext.RenderDestructured(configuration => configuration.Destructure.NetTopologySuiteTypes(), geometry);
        var withoutDestructuring = JointContext.RenderDestructured(_ => { }, geometry);

        withJoint.ShouldBe(withExtension);
        withJoint.ShouldNotBe(withoutDestructuring);
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is SpatialJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        typeof(SpatialJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private static async Task<IServiceProvider> Register(IServiceCollection services)
    {
        new SpatialJoint().Register(await JointContext.Create(), services);
        return services.BuildServiceProvider();
    }
}
