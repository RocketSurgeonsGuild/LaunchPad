using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.Primitives;

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
    public void Should_Turn_The_Exception_Into_Problem_Details(IExceptionFilter filter, Exception exception, int statusCode)
    {
        var context = CreateContext(exception);

        filter.OnException(context);

        context.ExceptionHandled.ShouldBeTrue();
        var result = context.Result.ShouldBeOfType<ObjectResult>();
        result.StatusCode.ShouldBe(statusCode);
        var problemDetails = result.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
        problemDetails.Detail.ShouldBe(exception.Message);
    }

    [Test]
    public async Task Should_Handle_The_Exception_Asynchronously_As_Well()
    {
        var context = CreateContext(new NotFoundException("no rocket"));

        await new NotFoundExceptionFilter(Factory()).OnExceptionAsync(context);

        context.ExceptionHandled.ShouldBeTrue();
        context.Result.ShouldBeOfType<ObjectResult>().StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Test]
    public void Should_Leave_Other_Exceptions_Alone()
    {
        var context = CreateContext(new NotAuthorizedException());

        new NotFoundExceptionFilter(Factory()).OnException(context);

        context.ExceptionHandled.ShouldBeFalse();
        context.Result.ShouldBeNull();
    }

    [Test]
    public void Should_Copy_The_Problem_Details_Of_The_Exception()
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

        var problemDetails = context.Result.ShouldBeOfType<ObjectResult>().Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Rocket not found");
        problemDetails.Type.ShouldBe("https://example.com/problems/not-found");
        problemDetails.Instance.ShouldBe("/rockets/1");
        problemDetails.Extensions["rocketId"].ShouldBe(1);
        // the factory's own extensions win over the exception's properties of the same name
        problemDetails.Extensions["traceId"].ShouldNotBe("from the exception");
    }

    [Test]
    public void Should_Let_A_Derived_Filter_Customize_The_Problem_Details()
    {
        var context = CreateContext(new NotFoundException("no rocket"));

        new TaggingFilter(Factory()).OnException(context);

        var result = context.Result.ShouldBeOfType<ObjectResult>();
        result.StatusCode.ShouldBe(StatusCodes.Status410Gone);
        result.Value.ShouldBeOfType<ProblemDetails>().Extensions["tag"].ShouldBe("custom");
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
