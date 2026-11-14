using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class JointOrderingTests
{
    [Test]
    public async Task Should_Run_The_Aspnetcore_Joint_Before_The_Joints_That_Depend_On_It()
    {
        // appended in the opposite order of the dependencies, so only the ordering attributes can put them right
        var context = await JointTestContext
                           .CreateBuilder()
                           .AppendJoint<SystemJsonTextJoint>()
                           .AppendJoint<ProblemDetailsJoint>()
                           .AppendJoint<OpenApiJoint>()
                           .AppendJoint<AspNetCoreJoint>()
                           .CreateAsync();

        var joints = context.Joints.Select(z => z.GetType()).ToList();

        var aspNetCore = joints.IndexOf(typeof(AspNetCoreJoint));
        await aspNetCore.Should().BeGreaterThanOrEqualTo(0);
        foreach (var dependent in new[]
                 {
                     typeof(SystemJsonTextJoint), typeof(ProblemDetailsJoint), typeof(OpenApiJoint),
                 })
        {
            await joints.IndexOf(dependent).Should().Because(dependent.Name).BeGreaterThan(aspNetCore);
        }
    }

    [Test]
    [Arguments(typeof(AspNetCoreJoint))]
    [Arguments(typeof(OpenApiJoint))]
    [Arguments(typeof(ProblemDetailsJoint))]
    [Arguments(typeof(SystemJsonTextJoint))]
    public async Task Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type(Type joint)
    {
        await ( await joint.GetCustomAttribute<DovetailCategoryAttribute>().Should().NotBeNull() )!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
        await Attribute.GetCustomAttributes(joint).Should().All(z => z is not ( LiveJointAttribute or UnitTestJointAttribute or DovetailHostTypeAttribute ));
    }
}
