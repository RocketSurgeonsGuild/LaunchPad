using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace AspNetCore.Tests.Joints;

public class SystemJsonTextJointTests
{
    [Test]
    public async Task Should_Apply_The_Existing_Json_Serializer_Options_To_The_Mvc_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
        services.Configure<JsonSerializerOptions>(options => options.WriteIndented = true);
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptions<MvcJsonOptions>>().Value.JsonSerializerOptions.WriteIndented.ShouldBeTrue();
    }

    [Test]
    public async Task Should_Apply_The_Existing_Json_Serializer_Options_To_The_Http_Json_Options()
    {
        var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
        services.Configure<JsonSerializerOptions>(options => options.WriteIndented = true);
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions.WriteIndented.ShouldBeTrue();
    }

    [Test]
    public async Task Should_Leave_The_Defaults_Alone_When_No_Json_Serializer_Options_Are_Configured()
    {
        var services = await JointTestContext.ApplyServicesAsync<SystemJsonTextJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IOptions<MvcJsonOptions>>().Value.JsonSerializerOptions.WriteIndented.ShouldBeFalse();
    }
}
