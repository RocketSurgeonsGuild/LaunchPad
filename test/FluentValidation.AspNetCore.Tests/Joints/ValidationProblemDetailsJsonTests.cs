using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using FluentValidation.Results;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace FluentValidation.AspNetCore.Tests.Joints;

// The validation problem details are written by the converter with the metadata the FluentValidationJoint chains, so they round trip once
// reflection is off, as in a native AOT application.
public class ValidationProblemDetailsJsonTests
{
    [Test]
    public async Task Should_Write_The_Validation_Problem_Details_Without_Reflection()
    {
        var options = WithoutReflection(await HttpOptions());

        var json = JsonSerializer.Serialize<object>(Problem(), options);

        json.ShouldContain("\"errors\":{\"Name\":[{\"propertyName\":\"Name\",\"errorMessage\":\"Name is required\"");
        json.ShouldContain("\"rules\":[\"default\"]");
        json.ShouldContain("\"traceId\":\"abc\"");
    }

    [Test]
    public async Task Should_Read_The_Validation_Problem_Details_Without_Reflection()
    {
        var options = WithoutReflection(await HttpOptions());

        var problem = JsonSerializer.Deserialize<FluentValidationProblemDetails>(JsonSerializer.Serialize(Problem(), options), options)!;

        problem.Title.ShouldBe("Validation failed");
        problem.Status.ShouldBe(400);
        problem.Rules.ShouldBe(["default"]);
        problem.ValidationErrors["Name"].ShouldHaveSingleItem().ErrorMessage.ShouldBe("Name is required");
        problem.Extensions.ShouldContainKey("traceId");
    }

    private static FluentValidationProblemDetails Problem()
    {
        var problem = new FluentValidationProblemDetails(new Results.ValidationResult([new ValidationFailure("Name", "Name is required", "") { ErrorCode = "NotEmpty" }]) { RuleSetsExecuted = ["default"] })
        {
            Title = "Validation failed",
            Status = 400,
        };
        problem.Extensions["traceId"] = "abc";
        return problem;
    }

    private static async Task<JsonSerializerOptions> HttpOptions()
    {
        var services = await JointTestContext.ApplyServicesAsync(
            JointTestContext.CreateBuilder().AppendJoint<SystemTextJsonJoint>().AppendJoint<SystemJsonTextJoint>().AppendJoint<Rocket.Surgery.LaunchPad.AspNetCore.Joints.FluentValidationJoint>()
        );
        using var provider = JointTestContext.BuildServiceProvider(services.AddOptions());
        return provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;
    }

    private static JsonSerializerOptions WithoutReflection(JsonSerializerOptions options) =>
        new(options) { TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolverChain.Where(x => x is not DefaultJsonTypeInfoResolver).ToArray()) };
}
