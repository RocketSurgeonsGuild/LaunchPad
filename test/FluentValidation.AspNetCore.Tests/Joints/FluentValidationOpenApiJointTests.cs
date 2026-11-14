using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;

namespace FluentValidation.AspNetCore.Tests.Joints;

public class FluentValidationOpenApiJointTests
{
    [Test]
    public async Task Is_Exported_As_An_Application_Joint_That_Runs_After_The_Open_Api_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationOpenApiJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(OpenApiJoint));
        await typeof(FluentValidationOpenApiJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Should_Register_The_Fluent_Validation_Open_Api_Rule_Handlers()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationOpenApiJoint>();

        // by name: the rule handlers are experimental
        await services.Count(d => d.ServiceType.Name == "IPropertyRuleHandler").Should().BeEqualTo(7);
    }
}
