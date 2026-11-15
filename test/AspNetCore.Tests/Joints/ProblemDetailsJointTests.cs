using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class ProblemDetailsJointTests
{
    [Test]
    public async Task Should_Not_Register_The_Validation_Problem_Details_Writers()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var writers = provider.GetServices<IProblemDetailsWriter>().Select(z => z.GetType().Name).ToArray();

        // the validation writers come from Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore
        await writers.Should().NotContain("FluentValidationProblemDetailsWriter");
        await writers.Should().NotContain("ValidationExceptionProblemDetailsWriter");
    }

    [Test]
    public async Task Should_Suppress_The_Model_State_Invalid_Filter()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value.SuppressModelStateInvalidFilter.Should().BeTrue();
    }
}
