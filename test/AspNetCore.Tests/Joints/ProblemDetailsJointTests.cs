using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.Primitives;

using ValidationException = FluentValidation.ValidationException;

namespace AspNetCore.Tests.Joints;

public class ProblemDetailsJointTests
{
    [Test]
    [Arguments(typeof(NotFoundException), StatusCodes.Status404NotFound)]
    [Arguments(typeof(RequestFailedException), StatusCodes.Status400BadRequest)]
    [Arguments(typeof(NotAuthorizedException), StatusCodes.Status403Forbidden)]
    [Arguments(typeof(ValidationException), StatusCodes.Status422UnprocessableEntity)]
    [Arguments(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError)]
    public async Task Should_Map_The_Exception_To_A_Status_Code(Type exceptionType, int expected)
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);
        var exception = (Exception)Activator.CreateInstance(exceptionType, "message")!;

        var selector = provider.GetRequiredService<IOptions<ExceptionHandlerOptions>>().Value.StatusCodeSelector;

        selector.ShouldNotBeNull();
        selector(exception).ShouldBe(expected);
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

        selector.ShouldNotBeNull();
        selector(new InvalidOperationException()).ShouldBe(StatusCodes.Status418ImATeapot);
        selector(new NotFoundException()).ShouldBe(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task Should_Register_The_Problem_Details_Writers()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var writers = provider.GetServices<IProblemDetailsWriter>().Select(z => z.GetType()).ToArray();

        writers.ShouldContain(typeof(OnBeforeWriteProblemDetailsWriter));
        writers.ShouldContain(typeof(FluentValidationProblemDetailsWriter));
        writers.ShouldContain(typeof(ValidationExceptionProblemDetailsWriter));
    }

    [Test]
    public async Task Should_Not_Register_A_Writer_Twice_When_The_Joint_Is_Applied_Twice()
    {
        var builder = JointTestContext.CreateBuilder().AppendJoint<ProblemDetailsJoint>();
        var context = await builder.CreateAsync();
        var services = new ServiceCollection();

        await services.ApplyService(context);
        await services.ApplyService(context);

        services
           .Count(d => d.ServiceType == typeof(IProblemDetailsWriter) && d.ImplementationType == typeof(FluentValidationProblemDetailsWriter))
           .ShouldBe(1);
    }

    [Test]
    public async Task Should_Suppress_The_Model_State_Invalid_Filter()
    {
        var services = await JointTestContext.ApplyServicesAsync<ProblemDetailsJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value.SuppressModelStateInvalidFilter.ShouldBeTrue();
    }
}
