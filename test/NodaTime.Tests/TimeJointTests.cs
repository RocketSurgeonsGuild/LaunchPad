using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Microsoft.Extensions.DependencyInjection;

using NodaTime.Testing.TimeZones;

using Rocket.Surgery.LaunchPad.NodaTime;
using Rocket.Surgery.LaunchPad.NodaTime.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.Rivets;

namespace NodaTime.Tests;

public class TimeJointTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_Register_The_System_Clock()
    {
        var provider = await CreateProvider(builder => builder);

        provider.GetRequiredService<TimeProvider>().ShouldBe(TimeProvider.System);
        provider.GetRequiredService<IClock>().ShouldBe(SystemClock.Instance);
        provider.GetRequiredService<IDateTimeZoneProvider>().VersionId.ShouldBe(DateTimeZoneProviders.Tzdb.VersionId);
    }

    [Test]
    public async Task Should_Read_The_NodaTime_Options_From_The_Context()
    {
        var source = new FakeDateTimeZoneSource.Builder { VersionId = "launchpad-test", }.Build();

        var provider = await CreateProvider(builder => builder.Set(new NodaTimeOptions { DateTimeZoneSource = source, }));

        provider.GetRequiredService<IDateTimeZoneProvider>().VersionId.ShouldBe("launchpad-test");
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeExports.Joints().Single(x => x.Joint is TimeJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(TimeJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private static async Task<IServiceProvider> CreateProvider(Func<DovetailContextBuilder, DovetailContextBuilder> configure)
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), DovetailHostType.UnitTest, []);
        builder.AppendJoint<TimeJoint>();

        var services = await TestJoints.ApplyServicesAsync(await DovetailContext.FromAsync(configure(builder)));
        return services.BuildServiceProvider();
    }
}
