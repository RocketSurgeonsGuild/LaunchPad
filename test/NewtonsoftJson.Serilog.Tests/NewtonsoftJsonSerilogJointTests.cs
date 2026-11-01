using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Linq;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Rivets;

namespace NewtonsoftJson.Serilog.Tests;

public class NewtonsoftJsonSerilogJointTests
{
    [Test]
    public async Task Destructures_A_JObject_As_A_Structure()
    {
        var context = await JointContext.Create();

        JointContext
           .RenderDestructured(
                configuration => new NewtonsoftJsonSerilogJoint().Register(context, new ServiceCollection(), configuration),
                JObject.FromObject(new { test = true, system = new { data = "1234" } })
            )
           .ShouldBe("Value { test: True, system: { data: \"1234\" } }");
    }

    [Test]
    public async Task Destructures_The_Newtonsoft_Json_Types_As_The_Serilog_Extension_Does()
    {
        var context = await JointContext.Create();
        var value = JObject.FromObject(new { test = true, system = new { data = "1234" } });

        var withJoint = JointContext.RenderDestructured(configuration => new NewtonsoftJsonSerilogJoint().Register(context, new ServiceCollection(), configuration), value);
        var withExtension = JointContext.RenderDestructured(configuration => configuration.Destructure.NewtonsoftJsonTypes(), value);
        var withoutDestructuring = JointContext.RenderDestructured(_ => { }, value);

        withJoint.ShouldBe(withExtension);
        withJoint.ShouldNotBe(withoutDestructuring);
    }

    [Test]
    public void Is_Exported_As_A_Core_Serilog_Joint_Without_Ordering_Or_Gating()
    {
        var export = NewtonsoftJsonSerilogExports.Joints().Single(x => x.Joint is NewtonsoftJsonSerilogJoint);

        export.Joint.ShouldBeAssignableTo<ISerilogJoint>();
        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        typeof(NewtonsoftJsonSerilogJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    [Test]
    public void The_Newtonsoft_Json_Joint_Is_No_Longer_A_Serilog_Joint()
    {
        typeof(NewtonsoftJsonJoint).IsAssignableTo(typeof(ISerilogJoint)).ShouldBeFalse();
    }
}
