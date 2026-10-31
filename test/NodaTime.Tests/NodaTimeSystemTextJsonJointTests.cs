using System.Reflection;
using System.Text.Json;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.NodaTime;
using Rocket.Surgery.LaunchPad.NodaTime.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.Rivets;

namespace NodaTime.Tests;

public class NodaTimeSystemTextJsonJointTests
{
    [Test]
    public async Task Writes_NodaTime_Values_In_The_LaunchPad_Formats()
    {
        var options = await Options(DateTimeZoneProviders.Tzdb);

        JsonSerializer.Serialize(Instant.FromUtc(2020, 1, 1, 12, 12, 12), options).ShouldBe("\"2020-01-01T12:12:12Z\"");
        JsonSerializer.Serialize(new LocalDate(2020, 1, 1), options).ShouldBe("\"2020-01-01\"");
    }

    [Test]
    public async Task Reads_The_Extra_Formats_That_The_LaunchPad_Converters_Accept()
    {
        var options = await Options(DateTimeZoneProviders.Tzdb);

        // The converter NodaTime ships rejects date time offset text for an instant, the composite one falls back to it.
        JsonSerializer.Deserialize<Instant>("\"01/01/2020 12:12:12 +00:00\"", options).ShouldBe(Instant.FromUtc(2020, 1, 1, 12, 12, 12));
    }

    [Test]
    public async Task Resolves_Time_Zones_Through_The_Registered_Time_Zone_Provider()
    {
        var provider = TestProvider();
        var options = await Options(provider);

        JsonSerializer.Deserialize<DateTimeZone>($"\"{TestZoneId}\"", options).ShouldBeSameAs(provider[TestZoneId]);
    }

    [Test]
    public async Task Does_Not_Care_Whether_The_Time_Zone_Provider_Is_Registered_Before_Or_After_It()
    {
        var provider = TestProvider();
        var services = new ServiceCollection().AddOptions();
        new NodaTimeSystemTextJsonJoint().Register(await JointContext(), services);
        services.AddSingleton<IDateTimeZoneProvider>(provider);

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        JsonSerializer.Deserialize<DateTimeZone>($"\"{TestZoneId}\"", options).ShouldBeSameAs(provider[TestZoneId]);
    }

    [Test]
    public async Task Uses_The_Time_Zone_Source_Of_The_NodaTime_Options_Through_The_Time_Joint()
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), DovetailHostType.UnitTest, []);
        builder.AppendJoint<TimeJoint>();
        builder.AppendJoint<NodaTimeSystemTextJsonJoint>();
        builder.Set(new NodaTimeOptions { DateTimeZoneSource = TestSource(), });
        var services = await TestJoints.ApplyServicesAsync(await DovetailContext.FromAsync(builder));
        // AddOptions comes from the Primitives DefaultJoint in an application.
        services.AddOptions();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        JsonSerializer.Deserialize<DateTimeZone>($"\"{TestZoneId}\"", options).ShouldBeSameAs(provider.GetRequiredService<IDateTimeZoneProvider>()[TestZoneId]);
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeExports.Joints().Single(x => x.Joint is NodaTimeSystemTextJsonJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(NodaTimeSystemTextJsonJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private const string TestZoneId = "UTC+03";

    private static FakeDateTimeZoneSource TestSource()
    {
        var builder = new FakeDateTimeZoneSource.Builder { VersionId = "launchpad-test", };
        builder.Add(DateTimeZone.ForOffset(Offset.FromHours(3)));
        return builder.Build();
    }

    private static DateTimeZoneCache TestProvider() => new(TestSource());

    private static async Task<JsonSerializerOptions> Options(IDateTimeZoneProvider dateTimeZoneProvider)
    {
        // AddOptions comes from the Primitives DefaultJoint in an application.
        var services = new ServiceCollection().AddOptions().AddSingleton(dateTimeZoneProvider);
        new NodaTimeSystemTextJsonJoint().Register(await JointContext(), services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
    }

    private static async Task<IDovetailContext> JointContext() => await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
