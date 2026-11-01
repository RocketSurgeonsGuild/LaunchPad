using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Serilog;
using Rocket.Surgery.LaunchPad.Serilog.Joints;
using Rocket.Surgery.LaunchPad.Serilog.Rivets;

namespace NodaTime.Serilog.Tests;

public class NodaTimeSerilogJointTests
{
    [Test]
    public async Task Destructures_NodaTime_Values_As_The_Text_Of_Their_Iso_Patterns()
    {
        var context = await JointContext.Create();

        JointContext
           .RenderDestructured(configuration => new NodaTimeSerilogJoint().Register(context, new ServiceCollection(), configuration), Instant.FromUtc(2022, 1, 1, 4, 4, 4))
           .ShouldBe("Value \"2022-01-01T04:04:04Z\"");
    }

    [Test]
    public async Task Destructures_The_NodaTime_Types_As_The_Serilog_Extension_Does()
    {
        var context = await JointContext.Create();
        var value = new Interval(Instant.FromUtc(2022, 1, 1, 4, 4, 4), Instant.FromUtc(2022, 1, 2, 4, 4, 4));

        var withJoint = JointContext.RenderDestructured(configuration => new NodaTimeSerilogJoint().Register(context, new ServiceCollection(), configuration), value);
        var withExtension = JointContext.RenderDestructured(configuration => configuration.Destructure.NodaTimeTypes(), value);
        var withoutDestructuring = JointContext.RenderDestructured(_ => { }, value);

        withJoint.ShouldBe(withExtension);
        withJoint.ShouldNotBe(withoutDestructuring);
    }

    [Test]
    public void Is_Exported_As_A_Core_Serilog_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeSerilogExports.Joints().Single(x => x.Joint is NodaTimeSerilogJoint);

        export.Joint.ShouldBeAssignableTo<ISerilogJoint>();
        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        typeof(NodaTimeSerilogJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }
}
