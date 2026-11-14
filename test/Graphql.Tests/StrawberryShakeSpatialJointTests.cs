using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Rivets;
using ISerializer = StrawberryShake.Serialization.ISerializer;

namespace Graphql.Tests;

public class StrawberryShakeSpatialJointTests
{
    [Test]
    public async Task Registers_The_Geometry_Serializer_As_A_Singleton()
    {
        var services = new ServiceCollection();
        new StrawberryShakeSpatialJoint().Register(await JointContext.Create(), services);

        await services.Where(x => x.ServiceType == typeof(ISerializer)).Select(x => (x.ImplementationType, x.Lifetime)).Should().BeEquivalentTo([(typeof(GeometrySerializer), ServiceLifetime.Singleton)], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is StrawberryShakeSpatialJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        await typeof(StrawberryShakeSpatialJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }
}
