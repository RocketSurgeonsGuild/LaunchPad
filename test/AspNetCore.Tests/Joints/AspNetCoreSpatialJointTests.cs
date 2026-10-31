using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class AspNetCoreSpatialJointTests
{
    [Test]
    public async Task Should_Not_Register_Any_Services()
    {
        var services = await JointTestContext.ApplyServicesAsync<AspNetCoreSpatialJoint>();

        services.ShouldBeEmpty();
    }

    [Test]
    public void Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type()
    {
        typeof(AspNetCoreSpatialJoint).GetCustomAttribute<DovetailCategoryAttribute>().ShouldNotBeNull().Category.ToString().ShouldBe(DovetailCategory.Application);
        Attribute
           .GetCustomAttributes(typeof(AspNetCoreSpatialJoint))
           .ShouldNotContain(z => z is LiveJointAttribute || z is UnitTestJointAttribute || z is DovetailHostTypeAttribute);
    }
}
