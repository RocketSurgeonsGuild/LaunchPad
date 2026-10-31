using Joinery.Dovetail;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;
using NodaTime.Testing.TimeZones;

using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Joints;

namespace Extensions.Tests;

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
    public async Task Should_Read_The_Primitive_Options_From_The_Context()
    {
        var source = new FakeDateTimeZoneSource.Builder { VersionId = "launchpad-test", }.Build();

        var provider = await CreateProvider(builder => builder.Set(new PrimitiveOptions { DateTimeZoneSource = source, }));

        provider.GetRequiredService<IDateTimeZoneProvider>().VersionId.ShouldBe("launchpad-test");
    }

    private static async Task<IServiceProvider> CreateProvider(Func<DovetailContextBuilder, DovetailContextBuilder> configure)
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), DovetailHostType.UnitTest, []);
        builder.AppendJoint<TimeJoint>();

        var services = await TestJoints.ApplyServicesAsync(await DovetailContext.FromAsync(configure(builder)));
        return services.BuildServiceProvider();
    }
}
