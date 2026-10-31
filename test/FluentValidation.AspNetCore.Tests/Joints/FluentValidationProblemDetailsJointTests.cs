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
using Rocket.Surgery.LaunchPad.Primitives;

namespace FluentValidation.AspNetCore.Tests.Joints;

public class FluentValidationProblemDetailsJointTests
{
    [Test]
    public void Is_Exported_As_An_Application_Joint_That_Runs_After_The_Problem_Details_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationProblemDetailsJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(ProblemDetailsJoint))
        );
        typeof(FluentValidationProblemDetailsJoint)
           .GetCustomAttribute<DovetailCategoryAttribute>()!
           .Category.ToString()
           .ShouldBe(DovetailCategory.Application);
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

        selector.ShouldNotBeNull();
        selector(exception).ShouldBe(expected);
    }

    [Test]
    public async Task Should_Register_The_Validation_Writers_After_The_Problem_Details_Writers()
    {
        var services = await JointTestContext.ApplyServicesAsync(
            JointTestContext.CreateBuilder().AppendJoint<FluentValidationProblemDetailsJoint>().AppendJoint<ProblemDetailsJoint>()
        );
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider
           .GetServices<IProblemDetailsWriter>()
           .Select(z => z.GetType().Name)
           .ShouldBe(
                [
                    "DefaultProblemDetailsWriter",
                    "OnBeforeWriteProblemDetailsWriter",
                    nameof(FluentValidationProblemDetailsWriter),
                    nameof(ValidationExceptionProblemDetailsWriter),
                ]
            );
    }

    [Test]
    public async Task Should_Not_Register_A_Writer_Twice_When_The_Joint_Is_Applied_Twice()
    {
        var context = await JointTestContext.CreateBuilder().AppendJoint<FluentValidationProblemDetailsJoint>().CreateAsync();
        var services = new ServiceCollection();

        await services.ApplyService(context);
        await services.ApplyService(context);

        services
           .Count(d => d.ServiceType == typeof(IProblemDetailsWriter) && d.ImplementationType == typeof(FluentValidationProblemDetailsWriter))
           .ShouldBe(1);
    }
}
