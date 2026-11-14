using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.Foundation;

namespace AspNetCore.Tests.Filters;

public class ProblemDetailsExceptionFilterTests
{
    public static IEnumerable<Func<(IExceptionFilter Filter, Exception Exception, int StatusCode)>> Filters()
    {
        yield return () => (new NotFoundExceptionFilter(Factory()), new NotFoundException("no rocket"), StatusCodes.Status404NotFound);
        yield return () => (new NotAuthorizedExceptionFilter(Factory()), new NotAuthorizedException("not yours"), StatusCodes.Status403Forbidden);
        yield return () => (new RequestFailedExceptionFilter(Factory()), new RequestFailedException("bad request"), StatusCodes.Status400BadRequest);
    }

    [Test]
    [MethodDataSource(nameof(Filters))]
    public async Task Should_Turn_The_Exception_Into_Problem_Details(IExceptionFilter filter, Exception exception, int statusCode)
    {
        var context = CreateContext(exception);

        filter.OnException(context);

        await context.ExceptionHandled.Should().BeTrue();
        var result = await Assert.That(context.Result).IsTypeOf<ObjectResult>();
        await result.StatusCode.Should().BeEqualTo(statusCode);
        var problemDetails = await Assert.That(result.Value).IsTypeOf<ProblemDetails>();
        await problemDetails.Status.Should().BeEqualTo(statusCode);
        await problemDetails.Detail.Should().BeEqualTo(exception.Message);
    }

    [Test]
    public async Task Should_Handle_The_Exception_Asynchronously_As_Well()
    {
        var context = CreateContext(new NotFoundException("no rocket"));

        await new NotFoundExceptionFilter(Factory()).OnExceptionAsync(context);

        await context.ExceptionHandled.Should().BeTrue();
        await ( await Assert.That(context.Result).IsTypeOf<ObjectResult>() )!.StatusCode.Should().BeEqualTo(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task Should_Leave_Other_Exceptions_Alone()
    {
        var context = CreateContext(new NotAuthorizedException());

        new NotFoundExceptionFilter(Factory()).OnException(context);

        await context.ExceptionHandled.Should().BeFalse();
        await context.Result.Should().BeNull();
    }

    [Test]
    public async Task Should_Copy_The_Problem_Details_Of_The_Exception()
    {
        var context = CreateContext(
            new NotFoundException("no rocket")
            {
                Title = "Rocket not found",
                Link = "https://example.com/problems/not-found",
                Instance = "/rockets/1",
                Properties = { ["rocketId"] = 1, ["traceId"] = "from the exception", },
            }
        );

        new NotFoundExceptionFilter(Factory()).OnException(context);

        var problemDetails = await Assert.That(( await Assert.That(context.Result).IsTypeOf<ObjectResult>() )!.Value).IsTypeOf<ProblemDetails>();
        await problemDetails.Title.Should().BeEqualTo("Rocket not found");
        await problemDetails.Type.Should().BeEqualTo("https://example.com/problems/not-found");
        await problemDetails.Instance.Should().BeEqualTo("/rockets/1");
        await problemDetails.Extensions["rocketId"].Should().BeEqualTo(1);
        // the factory's own extensions win over the exception's properties of the same name
        await problemDetails.Extensions["traceId"].Should().NotBeEqualTo("from the exception");
    }

    [Test]
    public async Task Should_Let_A_Derived_Filter_Customize_The_Problem_Details()
    {
        var context = CreateContext(new NotFoundException("no rocket"));

        new TaggingFilter(Factory()).OnException(context);

        var result = await Assert.That(context.Result).IsTypeOf<ObjectResult>();
        await result.StatusCode.Should().BeEqualTo(StatusCodes.Status410Gone);
        await ( await Assert.That(result.Value).IsTypeOf<ProblemDetails>() )!.Extensions["tag"].Should().BeEqualTo("custom");
    }

    private static ProblemDetailsFactory Factory() =>
        new ServiceCollection().AddLogging().AddControllers().Services.BuildServiceProvider().GetRequiredService<ProblemDetailsFactory>();

    private static ExceptionContext CreateContext(Exception exception) =>
        new(new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []) { Exception = exception, };

    private sealed class TaggingFilter(ProblemDetailsFactory factory) : ProblemDetailsExceptionFilter<NotFoundException>(StatusCodes.Status410Gone, factory)
    {
        protected override ProblemDetails CustomizeProblemDetails(ProblemDetails problemDetails)
        {
            problemDetails.Extensions["tag"] = "custom";
            return problemDetails;
        }
    }
}
