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

        await provider.GetRequiredService<TimeProvider>().Should().BeEqualTo(TimeProvider.System);
        await provider.GetRequiredService<IClock>().Should().BeEqualTo(SystemClock.Instance);
        await provider.GetRequiredService<IDateTimeZoneProvider>().VersionId.Should().BeEqualTo(DateTimeZoneProviders.Tzdb.VersionId);
    }

    [Test]
    public async Task Should_Read_The_NodaTime_Options_From_The_Context()
    {
        var source = new FakeDateTimeZoneSource.Builder { VersionId = "launchpad-test", }.Build();

        var provider = await CreateProvider(builder => builder.Set(new NodaTimeOptions { DateTimeZoneSource = source, }));

        await provider.GetRequiredService<IDateTimeZoneProvider>().VersionId.Should().BeEqualTo("launchpad-test");
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeExports.Joints().Single(x => x.Joint is TimeJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
        await typeof(TimeJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }

    private static async Task<IServiceProvider> CreateProvider(Func<DovetailContextBuilder, DovetailContextBuilder> configure)
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), DovetailHostType.UnitTest, []);
        builder.AppendJoint<TimeJoint>();

        var services = await TestJoints.ApplyServicesAsync(await DovetailContext.FromAsync(configure(builder)));
        return services.BuildServiceProvider();
    }
}
