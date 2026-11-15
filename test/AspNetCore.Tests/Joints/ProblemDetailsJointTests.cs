using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class ProblemDetailsJointTests
{
    [Test]
    public async Task Should_Suppress_The_Model_State_Invalid_Filter()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value.SuppressModelStateInvalidFilter.Should().BeTrue();
    }
}
