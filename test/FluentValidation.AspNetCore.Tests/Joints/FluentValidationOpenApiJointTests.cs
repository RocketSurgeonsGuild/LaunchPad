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
    public void Is_Exported_As_An_Application_Joint_That_Runs_After_The_Open_Api_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationOpenApiJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(OpenApiJoint))
        );
        typeof(FluentValidationOpenApiJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Should_Register_The_Fluent_Validation_Open_Api_Rule_Handlers()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationOpenApiJoint>();

        // by name: the rule handlers are experimental
        services.Count(d => d.ServiceType.Name == "IPropertyRuleHandler").ShouldBe(7);
    }
}
