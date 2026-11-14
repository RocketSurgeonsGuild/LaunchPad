using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using NetTopologySuite.Geometries;

using Rocket.Surgery.LaunchPad.Spatial;
using Rocket.Surgery.LaunchPad.Spatial.Joints;
using Rocket.Surgery.LaunchPad.Spatial.Rivets;

namespace Spatial.Serilog.Tests;

public class SpatialSerilogJointTests
{
    [Test]
    public async Task Destructures_A_Geometry_As_Its_Well_Known_Text()
    {
        var context = await JointContext.Create();

        await JointContext
           .RenderDestructured(configuration => new SpatialSerilogJoint().Register(context, new ServiceCollection(), configuration), new Point(30, 10))
           .Should().BeEqualTo("Value \"POINT (30 10)\"");
    }

    [Test]
    public async Task Destructures_The_NetTopologySuite_Types_As_The_Serilog_Extension_Does()
    {
        var context = await JointContext.Create();
        var geometry = new Point(30, 10);

        var withJoint = JointContext.RenderDestructured(configuration => new SpatialSerilogJoint().Register(context, new ServiceCollection(), configuration), geometry);
        var withExtension = JointContext.RenderDestructured(configuration => configuration.Destructure.NetTopologySuiteTypes(), geometry);
        var withoutDestructuring = JointContext.RenderDestructured(_ => { }, geometry);

        await withJoint.Should().BeEqualTo(withExtension);
        await withJoint.Should().NotBeEqualTo(withoutDestructuring);
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Serilog_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is SpatialSerilogJoint);

        await Assert.That(export.Joint).IsAssignableTo<ISerilogJoint>();
        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await typeof(SpatialSerilogJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }

    [Test]
    public async Task The_Spatial_Joint_Is_No_Longer_A_Serilog_Joint() => await typeof(SpatialJoint).IsAssignableTo(typeof(ISerilogJoint)).Should().BeFalse();
}
