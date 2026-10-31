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
    public async Task Should_Register_The_Fluent_Validation_Open_Api_Rule_Handlers()
    {
        var services = await JointTestContext.ApplyServicesAsync<OpenApiJoint>();

        // by name: the open api package marks its types as experimental
        services.Count(d => d.ServiceType.Name == "IPropertyRuleHandler").ShouldBe(7);
    }
}
