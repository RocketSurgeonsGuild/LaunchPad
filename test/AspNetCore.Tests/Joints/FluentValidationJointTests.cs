using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace AspNetCore.Tests.Joints;

public class FluentValidationJointTests
{
    [Test]
    public async Task Should_Put_The_Validation_Exception_Filter_First()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptions<MvcOptions>>().Value.Filters[0].ShouldBeOfType<ValidationExceptionFilter>();
    }

    [Test]
    public async Task Should_Add_The_Validation_Problem_Details_Converter_To_The_Mvc_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider
           .GetRequiredService<IOptions<MvcJsonOptions>>()
           .Value
           .JsonSerializerOptions
           .Converters
           .OfType<ValidationProblemDetailsConverter>()
           .ShouldHaveSingleItem();
    }

    [Test]
    public async Task Should_Add_The_Validation_Problem_Details_Converter_To_The_Http_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider
           .GetRequiredService<IOptions<HttpJsonOptions>>()
           .Value
           .SerializerOptions
           .Converters
           .OfType<ValidationProblemDetailsConverter>()
           .ShouldHaveSingleItem();
    }
}
