using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Sample.Classic.Restful.Tests;

public class ClassicRestfulConventionTests
{
    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
    public void Should_Have_Success_Response_Types(ApiDescriptionData description)
    {
        description.Description.SupportedResponseTypes.Should().Contain(z => z.StatusCode >= 200 && z.StatusCode < 300);
    }

    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
    public void Should_Have_Not_Found_Responses(ApiDescriptionData description)
    {
        var method = ( description.Description.ActionDescriptor as ControllerActionDescriptor )!.MethodInfo;
        if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)) return;
        description.Description.SupportedResponseTypes.Should().Contain(z => z.StatusCode == StatusCodes.Status404NotFound);
    }

    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
    public void Should_Have_Validation_Responses(ApiDescriptionData description)
    {
        description.Description.SupportedResponseTypes.Should().Contain(z => z.StatusCode == StatusCodes.Status422UnprocessableEntity);
    }

    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
    public void Should_Have_Bad_Request_Responses(ApiDescriptionData description)
    {
        description.Description.SupportedResponseTypes.Should().Contain(z => z.IsDefaultResponse);
    }
}
