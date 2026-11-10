using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

namespace ZeroAlloc.AspNetCore.Tests;

public class ZeroAllocProblemDetailsJointTests
{
    [Test]
    public void Is_Exported_As_An_Application_Joint_That_Runs_After_The_Problem_Details_Joint()
    {
        var export = ZeroAllocExports.Joints().Single(x => x.Joint is ZeroAllocProblemDetailsJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(ProblemDetailsJoint))
        );
        typeof(ZeroAllocProblemDetailsJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Maps_A_Validation_Failure_To_422_And_Keeps_The_Previous_Status_Codes()
    {
        var services = new ServiceCollection();
        services.AddExceptionHandler(options => options.StatusCodeSelector = _ => StatusCodes.Status404NotFound);
        new ZeroAllocProblemDetailsJoint().Register(await Context(), services);
        using var provider = services.BuildServiceProvider();

        var selector = provider.GetRequiredService<IOptions<ExceptionHandlerOptions>>().Value.StatusCodeSelector.ShouldNotBeNull();

        selector(Failures.Exception(("Name", "Name must not be empty."))).ShouldBe(StatusCodes.Status422UnprocessableEntity);
        selector(new InvalidOperationException()).ShouldBe(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task Registers_The_Problem_Details_Writer_Once()
    {
        var services = new ServiceCollection();
        var joint = new ZeroAllocProblemDetailsJoint();
        joint.Register(await Context(), services);
        joint.Register(await Context(), services);

        services
           .Count(d => d.ServiceType == typeof(IProblemDetailsWriter) && d.ImplementationType == typeof(ZeroAllocProblemDetailsJoint.ValidationFailureProblemDetailsWriter))
           .ShouldBe(1);
    }

    [Test]
    public void Answers_A_Controller_With_The_Failures_Grouped_By_Property()
    {
        var context = new ExceptionContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            []
        )
        {
            Exception = Failures.Exception(("Name", "Name must not be empty."), ("Name", "Name is too short."), ("Type", "Type is not valid.")),
        };

        new ZeroAllocProblemDetailsJoint.ValidationFailureExceptionFilter().OnException(context);

        context.ExceptionHandled.ShouldBeTrue();
        var details = context.Result.ShouldBeOfType<UnprocessableEntityObjectResult>().Value.ShouldBeOfType<ValidationFailureProblemDetails>();
        details.Status.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        details.Errors.Keys.ShouldBe(["Name", "Type"]);
        details.Errors["Name"].Select(x => x.ErrorMessage).ShouldBe(["Name must not be empty.", "Name is too short."]);
    }

    [Test]
    public void Leaves_Other_Exceptions_To_The_Other_Filters()
    {
        var context = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), [])
        {
            Exception = new InvalidOperationException(),
        };

        new ZeroAllocProblemDetailsJoint.ValidationFailureExceptionFilter().OnException(context);

        context.ExceptionHandled.ShouldBeFalse();
        context.Result.ShouldBeNull();
    }

    private static async Task<IDovetailContext> Context() =>
        await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
