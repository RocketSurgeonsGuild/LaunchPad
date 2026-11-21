using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using AspNetCore.Tests.Joints;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace AspNetCore.Tests.HealthChecks;

public class LaunchPadHealthCheckResponseWriterTests
{
    // The shape HealthChecks.UI reads: status, totalDuration and the entries, each with data, description, duration, exception, status and tags.
    [Test]
    public async Task Should_Write_The_Report_In_The_HealthChecks_UI_Shape()
    {
        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["db"] = new(HealthStatus.Healthy, "fine", TimeSpan.FromMilliseconds(12), null, new Dictionary<string, object> { ["note"] = "ok" }, ["ready"]),
                ["cache"] = new(HealthStatus.Degraded, "slow", TimeSpan.FromSeconds(1.5), null, null),
                ["queue"] = new(HealthStatus.Unhealthy, null, TimeSpan.FromSeconds(2), new InvalidOperationException("boom"), null),
            },
            TimeSpan.FromSeconds(3.5)
        );

        await Verify(Write(report), "json");
    }

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

        await LaunchPadHealthCheckResponseWriter.WriteAsync(context, report);

        await context.Response.ContentType.Should().BeEqualTo(LaunchPadHealthCheckResponseWriter.ContentType);
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        var written = document.RootElement.GetProperty("entries").GetProperty("db").GetProperty("data");
        await written.GetProperty("count").GetInt32().Should().BeEqualTo(3);
        await written.GetProperty("tags").EnumerateArray().Select(x => x.GetString()).Should().BeEquivalentTo(Tags, CollectionOrdering.Matching);
        await written.GetProperty("rocket").GetString().Should().BeEqualTo("Rocket { Name = falcon }");
    }

    private static string Write(HealthReport report)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            LaunchPadHealthCheckResponseWriter.Write(writer, report);
        }

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    private static readonly string[] Tags = ["a", "b"];

    private sealed record Rocket(string Name);
}
