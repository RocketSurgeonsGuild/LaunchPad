using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class OnBeforeWriteProblemDetailsWriterTests
{
    [Test]
    public void Should_Fill_In_The_Title_And_Type_Of_A_Known_Client_Error_And_Leave_The_Writing_To_Others()
    {
        var context = CreateContext(new() { Status = StatusCodes.Status404NotFound, });

        Writer().CanWrite(context).ShouldBeFalse();

        context.ProblemDetails.Title.ShouldBe("Not Found");
        context.ProblemDetails.Type.ShouldBe("https://tools.ietf.org/html/rfc9110#section-15.5.5");
    }

    [Test]
    public void Should_Keep_A_Title_And_Type_That_Are_Already_Set()
    {
        var context = CreateContext(new() { Status = StatusCodes.Status404NotFound, Title = "Rocket not found", Type = "https://example.com/not-found", });

        Writer().CanWrite(context);

        context.ProblemDetails.Title.ShouldBe("Rocket not found");
        context.ProblemDetails.Type.ShouldBe("https://example.com/not-found");
    }

    [Test]
    [Arguments(null)]
    [Arguments(StatusCodes.Status200OK)]
    public void Should_Leave_Problem_Details_Without_A_Known_Client_Error_Alone(int? status)
    {
        var context = CreateContext(new() { Status = status, });

        Writer().CanWrite(context).ShouldBeFalse();

        context.ProblemDetails.Title.ShouldBeNull();
        context.ProblemDetails.Type.ShouldBeNull();
    }

    [Test]
    public async Task Should_Never_Write()
    {
        await Should.ThrowAsync<NotImplementedException>(() => Writer().WriteAsync(CreateContext(new())).AsTask());
    }

    // ApiBehaviorOptions has no client error mapping until MVC configures it, which is what an application gets
    private static OnBeforeWriteProblemDetailsWriter Writer()
    {
        var options = new ApiBehaviorOptions();
        options.ClientErrorMapping[StatusCodes.Status404NotFound] = new() { Title = "Not Found", Link = "https://tools.ietf.org/html/rfc9110#section-15.5.5", };
        return new(Options.Create(options));
    }

    private static ProblemDetailsContext CreateContext(ProblemDetails problemDetails) => new() { HttpContext = new DefaultHttpContext(), ProblemDetails = problemDetails, };
}
