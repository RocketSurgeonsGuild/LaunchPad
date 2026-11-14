using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.Foundation;

namespace AspNetCore.Tests.Joints;

public class ProblemDetailsJointTests
{
    [Test]
    [Arguments(typeof(NotFoundException), StatusCodes.Status404NotFound)]
    [Arguments(typeof(RequestFailedException), StatusCodes.Status400BadRequest)]
    [Arguments(typeof(NotAuthorizedException), StatusCodes.Status403Forbidden)]
    [Arguments(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError)]
    public async Task Should_Map_The_Exception_To_A_Status_Code(Type exceptionType, int expected)
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);
        var exception = (Exception)Activator.CreateInstance(exceptionType, "message")!;

        var selector = provider.GetRequiredService<IOptions<ExceptionHandlerOptions>>().Value.StatusCodeSelector;

        await selector.Should().NotBeNull();
        await selector(exception).Should().BeEqualTo(expected);
    }

    [Test]
    public async Task Should_Keep_The_Status_Code_Selector_That_Was_Already_Configured()
    {
        var context = await JointTestContext.CreateBuilder().AppendJoint<ProblemDetailsJoint>().CreateAsync();
        var services = new ServiceCollection();
        services.Configure<ExceptionHandlerOptions>(options => options.StatusCodeSelector = _ => StatusCodes.Status418ImATeapot);
        await services.ApplyService(context);
        using var provider = JointTestContext.BuildServiceProvider(services);

        var selector = provider.GetRequiredService<IOptions<ExceptionHandlerOptions>>().Value.StatusCodeSelector;

        await selector.Should().NotBeNull();
        await selector(new InvalidOperationException()).Should().BeEqualTo(StatusCodes.Status418ImATeapot);
        await selector(new NotFoundException()).Should().BeEqualTo(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task Should_Register_The_Problem_Details_Writers()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var writers = provider.GetServices<IProblemDetailsWriter>().Select(z => z.GetType()).ToArray();

        await writers.Should().Contain(typeof(OnBeforeWriteProblemDetailsWriter));
        // the validation writers come from Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore
        await writers.Select(z => z.Name).Should().NotContain("FluentValidationProblemDetailsWriter");
        await writers.Select(z => z.Name).Should().NotContain("ValidationExceptionProblemDetailsWriter");
    }

    [Test]
    public async Task Should_Not_Register_A_Writer_Twice_When_The_Joint_Is_Applied_Twice()
    {
        var builder = JointTestContext.CreateBuilder().AppendJoint<ProblemDetailsJoint>();
        var context = await builder.CreateAsync();
        var services = new ServiceCollection();

        await services.ApplyService(context);
        await services.ApplyService(context);

        await services
           .Count(d => d.ServiceType == typeof(IProblemDetailsWriter) && d.ImplementationType == typeof(OnBeforeWriteProblemDetailsWriter))
           .Should().BeEqualTo(1);
    }

    [Test]
    public async Task Should_Suppress_The_Model_State_Invalid_Filter()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value.SuppressModelStateInvalidFilter.Should().BeTrue();
    }
}
