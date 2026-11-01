using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace FluentValidation.AspNetCore.Tests.Joints;

public class FluentValidationJointTests
{
    [Test]
    public void Is_Exported_As_An_Application_Joint_That_Runs_After_The_AspNetCore_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(AspNetCoreJoint))
        );
        typeof(FluentValidationJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Should_Describe_Validation_Failures_With_The_Fluent_Validation_Problem_Details()
    {
        var services = await JointTestContext.ApplyServicesAsync<FluentValidationJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptions<RestfulApiOptions>>().Value.ValidationProblemDetailsType.ShouldBe(typeof(FluentValidationProblemDetails));
    }

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
