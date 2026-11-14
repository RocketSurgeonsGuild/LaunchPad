using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace FluentValidation.AspNetCore.Tests.Joints;

public class FluentValidationJointTests
{
    [Test]
    public async Task Is_Exported_As_An_Application_Joint_That_Runs_After_The_AspNetCore_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(AspNetCoreJoint));
        await typeof(FluentValidationJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Should_Put_The_Validation_Exception_Filter_First()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<MvcOptions>>().Value.Filters[0].Should().BeOfType(typeof(ValidationExceptionFilter));
    }

    [Test]
    public async Task Should_Add_The_Validation_Problem_Details_Converter_To_The_Mvc_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider
           .GetRequiredService<IOptions<MvcJsonOptions>>()
           .Value
           .JsonSerializerOptions
           .Converters
           .OfType<ValidationProblemDetailsConverter>()
           .Should().HaveSingleItem();
    }

    [Test]
    public async Task Should_Add_The_Validation_Problem_Details_Converter_To_The_Http_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider
           .GetRequiredService<IOptions<HttpJsonOptions>>()
           .Value
           .SerializerOptions
           .Converters
           .OfType<ValidationProblemDetailsConverter>()
           .Should().HaveSingleItem();
    }
}
