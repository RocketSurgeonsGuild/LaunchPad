using System.Diagnostics;
using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Rocket.Surgery.LaunchPad.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.HotChocolate.Rivets;

namespace Graphql.Tests;

public class InstrumentationJointTests
{
    private const string HotChocolateSourceName = "HotChocolate.Diagnostics";

    [Test]
    public async Task Records_The_Hot_Chocolate_Activities()
    {
        var recorded = new List<string>();
        var services = ServicesRecording(recorded);
        new InstrumentationJoint().Register(await JointContext.Create(), new ServicesBuilder(services));

        RecordActivity(services);

        recorded.ShouldBe(["from-hot-chocolate"]);
    }

    [Test]
    public void Records_Nothing_From_The_Hot_Chocolate_Activity_Source_Without_The_Joint()
    {
        var recorded = new List<string>();
        var services = ServicesRecording(recorded);

        RecordActivity(services);

        recorded.ShouldBeEmpty();
    }

    [Test]
    public void Is_Exported_As_An_Application_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is InstrumentationJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Application);
        typeof(InstrumentationJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    // Activity sources are shared by the whole process, so what the tracer provider records is what shows that it listens to one.
    private static IServiceCollection ServicesRecording(List<string> recorded)
    {
        var services = new ServiceCollection().AddOptions().AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        new ServicesBuilder(services).WithTracing(tracing => tracing.AddProcessor(new RecordingProcessor(recorded)));
        return services;
    }

    private static void RecordActivity(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        using var tracerProvider = provider.GetRequiredService<TracerProvider>();
        using var source = new ActivitySource(HotChocolateSourceName);
        using var activity = source.StartActivity("from-hot-chocolate");
    }

    private sealed class RecordingProcessor(List<string> recorded) : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity data) => recorded.Add(data.OperationName);
    }

    private sealed class ServicesBuilder(IServiceCollection services) : IOpenTelemetryBuilder
    {
        public IServiceCollection Services { get; } = services;
    }
}
