using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using AspNetCore.Tests.Joints;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.Primitives.Joints;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace AspNetCore.Tests;

public class LaunchPadHelpersTests
{
    // The health check data is written with the HTTP JSON options' resolver chain, so the writer works with reflection off (native AOT): the
    // values LaunchPad's own context knows go as JSON, anything else as its ToString().
    [Test]
    public async Task Should_Write_Health_Check_Data_Without_Reflection()
    {
        var services = await JointTestContext.ApplyServicesAsync(JointTestContext.CreateBuilder().AppendJoint<SystemTextJsonJoint>().AppendJoint<SystemJsonTextJoint>());
        services.AddOptions().PostConfigure<HttpJsonOptions>(
            options => options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
                options.SerializerOptions.TypeInfoResolverChain.Where(x => x is not DefaultJsonTypeInfoResolver).ToArray()
            )
        );
        await using var provider = JointTestContext.BuildServiceProvider(services);
        var context = new DefaultHttpContext { RequestServices = provider, Response = { Body = new MemoryStream() } };
        var data = new Dictionary<string, object> { ["count"] = 3, ["tags"] = Tags, ["rocket"] = new Rocket("falcon") };
        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry> { ["db"] = new(HealthStatus.Healthy, "fine", TimeSpan.FromSeconds(1), null, data) },
            TimeSpan.FromSeconds(1)
        );

        await LaunchPadHelpers.DefaultResponseWriter(context, report);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        var written = document.RootElement.GetProperty("results").GetProperty("db").GetProperty("data");
        written.GetProperty("count").GetInt32().ShouldBe(3);
        written.GetProperty("tags").EnumerateArray().Select(x => x.GetString()).ShouldBe(Tags);
        written.GetProperty("rocket").GetString().ShouldBe("Rocket { Name = falcon }");
    }

    private static readonly string[] Tags = ["a", "b"];

    private sealed record Rocket(string Name);
}
