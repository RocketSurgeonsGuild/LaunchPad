using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace AspNetCore.Tests.Joints;

public class SystemJsonTextJointTests
{
    [Test]
    public async Task Should_Apply_The_Existing_Json_Serializer_Options_To_The_Mvc_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<MvcSystemJsonTextJoint>();
        services.Configure<JsonSerializerOptions>(options => options.WriteIndented = true);
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<MvcJsonOptions>>().Value.JsonSerializerOptions.WriteIndented.Should().BeTrue();
    }

    [Test]
    public async Task Should_Apply_The_Existing_Json_Serializer_Options_To_The_Http_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
        services.Configure<JsonSerializerOptions>(options => options.WriteIndented = true);
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions.WriteIndented.Should().BeTrue();
    }

    // A JIT application serializes through the same JSON contexts as a native AOT one: a type no context knows has no metadata.
    [Test]
    [NotInParallel(nameof(SystemJsonTextJoint.KeepReflectionSwitch))]
    public async Task Should_Take_AspNetCores_Reflection_Resolver_Out_Of_The_Http_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var options = provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;

        await options.TypeInfoResolverChain.Should().All(resolver => resolver is not DefaultJsonTypeInfoResolver);
        await options.TryGetTypeInfo(typeof(UnknownToEveryContext), out _).Should().BeFalse();
    }

    // AddProblemDetails combines ASP.NET Core's problem details context into the options, before or after the joint.
    [Test]
    [NotInParallel(nameof(SystemJsonTextJoint.KeepReflectionSwitch))]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Should_Take_AspNetCores_Reflection_Resolver_Out_With_Problem_Details(bool problemDetailsFirst)
    {
        IServiceCollection services = new ServiceCollection();
        if (problemDetailsFirst) services.AddProblemDetails();
        foreach (var service in await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>()) services.Add(service);
        if (!problemDetailsFirst) services.AddProblemDetails();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var options = provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;

        await options.TryGetTypeInfo(typeof(UnknownToEveryContext), out _).Should().BeFalse();
        await options.TryGetTypeInfo(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), out _).Should().BeTrue();
    }

    // The generated controllers answer 400 with ValidationProblemDetails, and OpenAPI reads its metadata from the HTTP JSON options.
    [Test]
    [NotInParallel(nameof(SystemJsonTextJoint.KeepReflectionSwitch))]
    public async Task Should_Give_The_Http_Json_Options_The_Metadata_Of_Mvcs_Problem_Details()
    {
        var services = await JointTestContext.ApplyServicesAsync(
            JointTestContext.CreateBuilder().AppendJoint<SystemTextJsonJoint>().AppendJoint<SystemJsonTextJoint>().AppendJoint<MvcSystemJsonTextJoint>()
        );
        using var provider = JointTestContext.BuildServiceProvider(services);

        var options = provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;

        await options.TryGetTypeInfo(typeof(Microsoft.AspNetCore.Mvc.ValidationProblemDetails), out _).Should().BeTrue();
    }

    [Test]
    [NotInParallel(nameof(SystemJsonTextJoint.KeepReflectionSwitch))]
    public async Task Should_Keep_A_Reflection_Resolver_The_Application_Adds()
    {
        var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
        var reflection = new DefaultJsonTypeInfoResolver();
        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Add(reflection));
        using var provider = JointTestContext.BuildServiceProvider(services);

        var options = provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;

        await options.TypeInfoResolverChain.Should().Contain(reflection);
        await options.TryGetTypeInfo(typeof(UnknownToEveryContext), out _).Should().BeTrue();
    }

    [Test]
    [NotInParallel(nameof(SystemJsonTextJoint.KeepReflectionSwitch))]
    public async Task Should_Keep_AspNetCores_Reflection_Resolver_When_The_Application_Opts_Out()
    {
        AppContext.SetSwitch(SystemJsonTextJoint.KeepReflectionSwitch, true);
        try
        {
            var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
            using var provider = JointTestContext.BuildServiceProvider(services);

            var options = provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;

            await options.TryGetTypeInfo(typeof(UnknownToEveryContext), out _).Should().BeTrue();
        }
        finally
        {
            AppContext.SetSwitch(SystemJsonTextJoint.KeepReflectionSwitch, false);
        }
    }

    [Test]
    public async Task Should_Leave_The_Defaults_Alone_When_No_Json_Serializer_Options_Are_Configured()
    {
        var services = await JointTestContext.ApplyServicesAsync<MvcSystemJsonTextJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        await provider.GetRequiredService<IOptions<MvcJsonOptions>>().Value.JsonSerializerOptions.WriteIndented.Should().BeFalse();
    }

    private sealed record UnknownToEveryContext(string Name);
}
