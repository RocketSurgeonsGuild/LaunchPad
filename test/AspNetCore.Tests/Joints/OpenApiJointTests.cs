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

        await provider.GetRequiredService<IOptionsMonitor<OpenApiOptions>>().Get("v1").DocumentName.Should().BeEqualTo("v1");
    }
}
