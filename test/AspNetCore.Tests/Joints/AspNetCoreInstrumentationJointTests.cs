using System.Diagnostics;
using System.Diagnostics.Metrics;

using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenTelemetry;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class AspNetCoreInstrumentationJointTests
{
    [Test]
    public async Task Should_Record_Exceptions_In_The_Aspnetcore_Traces()
    {
        using var provider = await BuildAsync();

        provider.GetRequiredService<IOptionsMonitor<AspNetCoreTraceInstrumentationOptions>>().CurrentValue.RecordException.ShouldBeTrue();
    }

    [Test]
    public async Task Should_Listen_To_The_Aspnetcore_Activity_Source()
    {
        using var provider = await BuildAsync();
        provider.GetRequiredService<TracerProvider>();

        using var source = new ActivitySource("Microsoft.AspNetCore");

        source.HasListeners().ShouldBeTrue();
    }

    [Test]
    public async Task Should_Listen_To_The_Aspnetcore_Meters()
    {
        using var provider = await BuildAsync();
        provider.GetRequiredService<MeterProvider>();

        using var meter = new Meter("Microsoft.AspNetCore.Hosting");
        var counter = meter.CreateCounter<long>("requests");

        counter.Enabled.ShouldBeTrue();
    }

    private static async Task<ServiceProvider> BuildAsync()
    {
        var context = await JointTestContext
                           .CreateBuilder()
                           .AppendJoint<AspNetCoreInstrumentationJoint>()
                           .CreateAsync();
        var services = new ServiceCollection();
        var builder = services.AddOpenTelemetry();
        await builder.ApplyOpenTelemetry(context);
        // The SDK only enables instruments once there is a reader to collect them.
        builder.WithMetrics(metrics => metrics.AddReader(new BaseExportingMetricReader(new NoopMetricExporter())));
        return JointTestContext.BuildServiceProvider(services);
    }

    private sealed class NoopMetricExporter : BaseExporter<Metric>
    {
        public override ExportResult Export(in Batch<Metric> _) => ExportResult.Success;
    }
}
