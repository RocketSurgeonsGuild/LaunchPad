﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Sample.Restful.Tests.Helpers;

namespace Sample.Restful.Tests;

public class RestfulConventionTests
{
    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebAppFixture>))]
    public void Should_Have_Success_Response_Types(ApiDescriptionData description)
    {
        description.Description.SupportedResponseTypes.ShouldContain(z => z.StatusCode >= 200 && z.StatusCode < 300);
    }

    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebAppFixture>))]
    public void Should_Have_Not_Found_Responses(ApiDescriptionData description)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var method = (description.Description.ActionDescriptor as ControllerActionDescriptor)!.MethodInfo;
        if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)) return;
        description.Description.SupportedResponseTypes.ShouldContain(z => z.StatusCode == StatusCodes.Status404NotFound);
    }

    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebAppFixture>))]
    public void Should_Have_Validation_Responses(ApiDescriptionData description)
    {
        description.Description.SupportedResponseTypes.ShouldContain(z => z.StatusCode == StatusCodes.Status422UnprocessableEntity);
    }

    [Theory]
    [ClassData(typeof(ApiDescriptionData<TestWebAppFixture>))]
    public void Should_Have_Bad_Request_Responses(ApiDescriptionData description)
    {
        description.Description.SupportedResponseTypes.ShouldContain(z => z.IsDefaultResponse);
    }
}
