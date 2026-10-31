using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Rivets;
using ISerializer = global::StrawberryShake.Serialization.ISerializer;

namespace Graphql.Tests;

public class StrawberryShakeSpatialJointTests
{
    [Test]
    public async Task Registers_The_Geometry_Serializer_As_A_Singleton()
    {
        var services = new ServiceCollection();
        new StrawberryShakeSpatialJoint().Register(await JointContext.Create(), services);

        services.Where(x => x.ServiceType == typeof(ISerializer)).Select(x => (x.ImplementationType, x.Lifetime)).ShouldBe(
            [(typeof(GeometrySerializer), ServiceLifetime.Singleton)]
        );
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is StrawberryShakeSpatialJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        typeof(StrawberryShakeSpatialJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }
}
