using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;
using Rocket.Surgery.LaunchPad.Foundation;

namespace FluentValidation.AspNetCore.Tests.Joints;

public class FluentValidationProblemDetailsJointTests
{
    [Test]
    public async Task Is_Exported_As_An_Application_Joint_That_Runs_After_The_Problem_Details_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationProblemDetailsJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(ProblemDetailsJoint));
        await typeof(FluentValidationProblemDetailsJoint)
           .GetCustomAttribute<DovetailCategoryAttribute>()!
           .Category.ToString()
           .Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    [Arguments(typeof(ValidationException), StatusCodes.Status422UnprocessableEntity)]
    [Arguments(typeof(NotFoundException), StatusCodes.Status404NotFound)]
    [Arguments(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError)]
    public async Task Should_Map_The_Validation_Exception_And_Keep_The_Problem_Details_Mappings(Type exceptionType, int expected)
    {
        var services = await JointTestContext.ApplyServicesAsync(
            JointTestContext.CreateBuilder().AppendJoint<FluentValidationProblemDetailsJoint>().AppendJoint<ProblemDetailsJoint>()
        );
        using var provider = JointTestContext.BuildServiceProvider(services);
        var exception = (Exception)Activator.CreateInstance(exceptionType, "message")!;

        var selector = provider.GetRequiredService<IOptions<ExceptionHandlerOptions>>().Value.StatusCodeSelector;

        await selector.Should().NotBeNull();
        await selector(exception).Should().BeEqualTo(expected);
    }

    [Test]
    public async Task Should_Register_The_Validation_Writers_After_The_Problem_Details_Writers()
    {
        var services = await JointTestContext.ApplyServicesAsync(
            JointTestContext.CreateBuilder().AppendJoint<FluentValidationProblemDetailsJoint>().AppendJoint<ProblemDetailsJoint>()
        );
        using var provider = JointTestContext.BuildServiceProvider(services);

        var writers = provider.GetServices<IProblemDetailsWriter>().Select(z => z.GetType().Name).ToList();

        await writers.IndexOf(nameof(FluentValidationProblemDetailsWriter)).Should().BeGreaterThan(writers.IndexOf("OnBeforeWriteProblemDetailsWriter"));
        await writers.IndexOf(nameof(ValidationExceptionProblemDetailsWriter)).Should().BeGreaterThan(writers.IndexOf(nameof(FluentValidationProblemDetailsWriter)));
        await writers.IndexOf("OnBeforeWriteProblemDetailsWriter").Should().BeGreaterThanOrEqualTo(0);
    }

    [Test]
    public async Task Should_Not_Register_A_Writer_Twice_When_The_Joint_Is_Applied_Twice()
    {
        var context = await JointTestContext.CreateBuilder().AppendJoint<FluentValidationProblemDetailsJoint>().CreateAsync();
        var services = new ServiceCollection();

        await services.ApplyService(context);
        await services.ApplyService(context);

        await services
           .Count(d => d.ServiceType == typeof(IProblemDetailsWriter) && d.ImplementationType == typeof(FluentValidationProblemDetailsWriter))
           .Should().BeEqualTo(1);
    }
}
