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
                           .AppendJoint<RestfulJoint>()
                           .AppendJoint<ProblemDetailsJoint>()
                           .AppendJoint<OpenApiJoint>()
                           .AppendJoint<AspNetCoreJoint>()
                           .CreateAsync();

        var joints = context.Joints.Select(z => z.GetType()).ToList();

        var aspNetCore = joints.IndexOf(typeof(AspNetCoreJoint));
        aspNetCore.ShouldBeGreaterThanOrEqualTo(0);
        foreach (var dependent in new[]
                 {
                     typeof(SystemJsonTextJoint), typeof(RestfulJoint), typeof(ProblemDetailsJoint), typeof(OpenApiJoint),
                 })
        {
            joints.IndexOf(dependent).ShouldBeGreaterThan(aspNetCore, dependent.Name);
        }
    }

    [Test]
    [Arguments(typeof(AspNetCoreJoint))]
    [Arguments(typeof(OpenApiJoint))]
    [Arguments(typeof(ProblemDetailsJoint))]
    [Arguments(typeof(RestfulJoint))]
    [Arguments(typeof(SystemJsonTextJoint))]
    public void Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type(Type joint)
    {
        joint.GetCustomAttribute<DovetailCategoryAttribute>().ShouldNotBeNull().Category.ToString().ShouldBe(DovetailCategory.Application);
        Attribute.GetCustomAttributes(joint).ShouldNotContain(z => z is LiveJointAttribute || z is UnitTestJointAttribute || z is DovetailHostTypeAttribute);
    }
}
