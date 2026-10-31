using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class OpenApiJointTests
{
    [Test]
    public async Task Should_Register_The_V1_Document()
    {
        var services = await JointTestContext.ApplyServicesAsync<OpenApiJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptionsMonitor<OpenApiOptions>>().Get("v1").DocumentName.ShouldBe("v1");
    }

    [Test]
    public async Task Should_Not_Register_The_Fluent_Validation_Open_Api_Rule_Handlers()
    {
        var services = await JointTestContext.ApplyServicesAsync<OpenApiJoint>();

        // they come from Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore
        services.ShouldNotContain(d => d.ServiceType.Name == "IPropertyRuleHandler");
    }
}
