using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Rocket.Surgery.LaunchPad.AspNetCore;

namespace AspNetCore.Tests.Restful;

public class RestfulApiOptionsTests
{
    [Test]
    public void Should_Describe_Validation_Failures_With_The_Validation_Problem_Details_By_Default()
    {
        var options = new RestfulApiOptions();

        options.ValidationStatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        options.ValidationProblemDetailsType.ShouldBe(typeof(ValidationProblemDetails));
    }
}
