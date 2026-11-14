using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.NodaTime;
using Rocket.Surgery.LaunchPad.NodaTime.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.Rivets;

namespace NodaTime.Serilog.Tests;

public class NodaTimeSerilogJointTests
{
    [Test]
    public async Task Destructures_NodaTime_Values_As_The_Text_Of_Their_Iso_Patterns()
    {
        var context = await JointContext.Create();

        await JointContext
           .RenderDestructured(configuration => new NodaTimeSerilogJoint().Register(context, new ServiceCollection(), configuration), Instant.FromUtc(2022, 1, 1, 4, 4, 4))
           .Should().BeEqualTo("Value \"2022-01-01T04:04:04Z\"");
    }

    [Test]
    public async Task Destructures_The_NodaTime_Types_As_The_Serilog_Extension_Does()
    {
        var context = await JointContext.Create();
        var value = new Interval(Instant.FromUtc(2022, 1, 1, 4, 4, 4), Instant.FromUtc(2022, 1, 2, 4, 4, 4));

        var withJoint = JointContext.RenderDestructured(configuration => new NodaTimeSerilogJoint().Register(context, new ServiceCollection(), configuration), value);
        var withExtension = JointContext.RenderDestructured(configuration => configuration.Destructure.NodaTimeTypes(), value);
        var withoutDestructuring = JointContext.RenderDestructured(_ => { }, value);

        await withJoint.Should().BeEqualTo(withExtension);
        await withJoint.Should().NotBeEqualTo(withoutDestructuring);
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Serilog_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeExports.Joints().Single(x => x.Joint is NodaTimeSerilogJoint);

        await Assert.That(export.Joint).IsAssignableTo<ISerilogJoint>();
        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await typeof(NodaTimeSerilogJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }
}
