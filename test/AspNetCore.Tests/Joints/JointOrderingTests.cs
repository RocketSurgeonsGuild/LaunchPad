using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

namespace AspNetCore.Tests.Joints;

public class JointOrderingTests
{
    [Test]
    public async Task Should_Run_Each_Mvc_Joint_After_The_AspNetCore_Joint_It_Extends()
    {
        // appended in the opposite order of the dependencies, so only the ordering attributes can put them right
        var context = await JointTestContext
                           .CreateBuilder()
                           .AppendJoint<MvcSystemJsonTextJoint>()
                           .AppendJoint<MvcProblemDetailsJoint>()
                           .AppendJoint<MvcSerilogRequestLoggingJoint>()
                           .AppendJoint<SystemJsonTextJoint>()
                           .AppendJoint<ProblemDetailsJoint>()
                           .AppendJoint<SerilogRequestLoggingJoint>()
                           .CreateAsync();

        var joints = context.Joints.Select(z => z.GetType()).ToList();

        foreach (var (aspNetCore, mvc) in new[]
                 {
                     (typeof(SystemJsonTextJoint), typeof(MvcSystemJsonTextJoint)),
                     (typeof(ProblemDetailsJoint), typeof(MvcProblemDetailsJoint)),
                     (typeof(SerilogRequestLoggingJoint), typeof(MvcSerilogRequestLoggingJoint)),
                 })
        {
            await joints.IndexOf(mvc).Should().Because(mvc.Name).BeGreaterThan(joints.IndexOf(aspNetCore));
        }
    }

    [Test]
    [Arguments(typeof(OpenApiJoint))]
    [Arguments(typeof(ProblemDetailsJoint))]
    [Arguments(typeof(SystemJsonTextJoint))]
    [Arguments(typeof(MvcJoint))]
    [Arguments(typeof(MvcProblemDetailsJoint))]
    [Arguments(typeof(MvcSystemJsonTextJoint))]
    [Arguments(typeof(MvcSerilogRequestLoggingJoint))]
    public async Task Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type(Type joint)
    {
        await ( await joint.GetCustomAttribute<DovetailCategoryAttribute>().Should().NotBeNull() )!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
        await Attribute.GetCustomAttributes(joint).Should().All(z => z is not ( LiveJointAttribute or UnitTestJointAttribute or DovetailHostTypeAttribute ));
    }
}
