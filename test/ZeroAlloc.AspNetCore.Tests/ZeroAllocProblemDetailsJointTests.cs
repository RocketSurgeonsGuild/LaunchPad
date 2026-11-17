using System.Reflection;
using System.Text.Json;

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
using Rocket.Surgery.LaunchPad.Foundation.Joints;

namespace ZeroAlloc.AspNetCore.Tests;

public class ZeroAllocProblemDetailsJointTests
{
    [Test]
    public async Task Is_Exported_As_An_Application_Joint_That_Runs_After_The_Problem_Details_Joint()
    {
        var export = ZeroAllocExports.Joints().Single(x => x.Joint is ZeroAllocProblemDetailsJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(ProblemDetailsJoint));
        await typeof(ZeroAllocProblemDetailsJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Maps_A_Validation_Failure_To_422_And_Keeps_The_Previous_Status_Codes()
    {
        var services = new ServiceCollection();
        services.AddExceptionHandler(options => options.StatusCodeSelector = _ => StatusCodes.Status404NotFound);
        new ZeroAllocProblemDetailsJoint().Register(await Context(), services);
        using var provider = services.BuildServiceProvider();

        var selector = await provider.GetRequiredService<IOptions<ExceptionHandlerOptions>>().Value.StatusCodeSelector.Should().NotBeNull();

        await selector(Failures.Exception(("Name", "Name must not be empty."))).Should().BeEqualTo(StatusCodes.Status422UnprocessableEntity);
        await selector(new InvalidOperationException()).Should().BeEqualTo(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task Registers_The_Problem_Details_Writer_Once()
    {
        var services = new ServiceCollection();
        var joint = new ZeroAllocProblemDetailsJoint();
        joint.Register(await Context(), services);
        joint.Register(await Context(), services);

        await services
           .Count(d => d.ServiceType == typeof(IProblemDetailsWriter) && d.ImplementationType == typeof(ZeroAllocProblemDetailsJoint.ValidationFailureProblemDetailsWriter))
           .Should().BeEqualTo(1);
    }

    [Test]
    public async Task Answers_A_Controller_With_The_Failures_Grouped_By_Property()
    {
        var context = new ExceptionContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            []
        )
        {
            Exception = Failures.Exception(("Name", "Name must not be empty."), ("Name", "Name is too short."), ("Type", "Type is not valid.")),
        };

        new ZeroAllocProblemDetailsJoint.ValidationFailureExceptionFilter().OnException(context);

        await context.ExceptionHandled.Should().BeTrue();
        var details = await Assert.That(( await Assert.That(context.Result).IsTypeOf<UnprocessableEntityObjectResult>() )!.Value).IsTypeOf<ValidationFailureProblemDetails>();
        await details.Status.Should().BeEqualTo(StatusCodes.Status422UnprocessableEntity);
        await details.Errors.Keys.Should().BeEquivalentTo(["Name", "Type"], CollectionOrdering.Matching);
        await details.Errors["Name"].Select(x => x.ErrorMessage).Should().BeEquivalentTo(["Name must not be empty.", "Name is too short."], CollectionOrdering.Matching);
    }

    // The writer serializes through the HTTP JSON options' JSON contexts: LaunchPad takes ASP.NET Core's reflection resolver out of them.
    [Test]
    public async Task Writes_The_Failures_Through_The_Json_Contexts()
    {
        var services = new ServiceCollection().AddLogging();
        var context = await Context();
        new SystemTextJsonJoint().Register(context, services);
        new SystemJsonTextJoint().Register(context, services);
        new ZeroAllocProblemDetailsJoint().Register(context, services);
        using var provider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = provider };
        using var body = new MemoryStream();
        httpContext.Response.Body = body;

        await new ZeroAllocProblemDetailsJoint.ValidationFailureProblemDetailsWriter().WriteAsync(
            new() { HttpContext = httpContext, Exception = Failures.Exception(("Name", "Name must not be empty.")), ProblemDetails = new() { Instance = "/rockets" } }
        );

        using var document = JsonDocument.Parse(body.ToArray());
        await httpContext.Response.StatusCode.Should().BeEqualTo(StatusCodes.Status422UnprocessableEntity);
        await document.RootElement.GetProperty("instance").GetString().Should().BeEqualTo("/rockets");
        await document.RootElement.GetProperty("errors").GetProperty("Name")[0].GetProperty("errorMessage").GetString().Should().BeEqualTo("Name must not be empty.");
    }

    [Test]
    public async Task Leaves_Other_Exceptions_To_The_Other_Filters()
    {
        var context = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), [])
        {
            Exception = new InvalidOperationException(),
        };

        new ZeroAllocProblemDetailsJoint.ValidationFailureExceptionFilter().OnException(context);

        await context.ExceptionHandled.Should().BeFalse();
        await context.Result.Should().BeNull();
    }

    private static async Task<IDovetailContext> Context() =>
        await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
