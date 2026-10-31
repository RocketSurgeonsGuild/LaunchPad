using Joinery.Dovetail;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class AspNetCoreValidationBehaviorJointTests
{
    [Test]
    public async Task Should_Ask_For_The_Validation_Options_To_Be_Registered_As_Health_Checks()
    {
        // Setup joints run while the context is created
        var context = await JointTestContext
                           .CreateBuilder()
                           .AppendJoint<AspNetCoreValidationBehaviorJoint>()
                           .CreateAsync();

        context.Get<bool>(AspNetCoreValidationBehaviorJoint.RegisterValidationOptionsAsHealthChecksKey).ShouldBeTrue();
    }

    [Test]
    public async Task Should_Not_Ask_For_It_When_The_Joint_Is_Not_Part_Of_The_Context()
    {
        var context = await JointTestContext.CreateBuilder().CreateAsync();

        context.Get<bool>(AspNetCoreValidationBehaviorJoint.RegisterValidationOptionsAsHealthChecksKey).ShouldBeFalse();
    }
}
