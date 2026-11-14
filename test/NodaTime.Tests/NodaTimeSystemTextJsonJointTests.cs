using System.Reflection;
using System.Text.Json;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

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

        await JsonSerializer.Serialize(Instant.FromUtc(2020, 1, 1, 12, 12, 12), options).Should().BeEqualTo("\"2020-01-01T12:12:12Z\"");
        await JsonSerializer.Serialize(new LocalDate(2020, 1, 1), options).Should().BeEqualTo("\"2020-01-01\"");
    }

    // Native AOT turns reflection off: the NodaTime types then resolve from the converters alone, also as an object or a nullable.
    [Test]
    public async Task Writes_NodaTime_Values_Without_Reflection()
    {
        var options = await Options(DateTimeZoneProviders.Tzdb);
        options = new(options)
        {
            TypeInfoResolver = System.Text.Json.Serialization.Metadata.JsonTypeInfoResolver.Combine(
                options.TypeInfoResolverChain.Where(x => x is not System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver).ToArray()
            ),
        };

        await JsonSerializer.Serialize<object>(Instant.FromUtc(2020, 1, 1, 12, 12, 12), options).Should().BeEqualTo("\"2020-01-01T12:12:12Z\"");
        await JsonSerializer.Serialize<LocalDate?>(new LocalDate(2020, 1, 1), options).Should().BeEqualTo("\"2020-01-01\"");
        await JsonSerializer.Deserialize<Duration?>("null", options).Should().BeNull();
        await JsonSerializer.Deserialize<DateTimeZone>("\"Europe/London\"", options)!.Id.Should().BeEqualTo("Europe/London");
    }

    [Test]
    public async Task Chains_Only_Its_Own_Resolver_When_Configured_Standalone()
    {
        var options = new JsonSerializerOptions().ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

        await options.TypeInfoResolverChain.Should().BeEquivalentTo([NodaTimeSystemTextJsonSerializationExtensions.TypeInfoResolver], CollectionOrdering.Matching);
        await JsonSerializer.Serialize(new LocalDate(2020, 1, 1), options).Should().BeEqualTo("\"2020-01-01\"");
    }

    [Test]
    public async Task Reads_The_Extra_Formats_That_The_LaunchPad_Converters_Accept()
    {
        var options = await Options(DateTimeZoneProviders.Tzdb);

        // The converter NodaTime ships rejects date time offset text for an instant, the composite one falls back to it.
        await JsonSerializer.Deserialize<Instant>("\"01/01/2020 12:12:12 +00:00\"", options).Should().BeEqualTo(Instant.FromUtc(2020, 1, 1, 12, 12, 12));
    }

    [Test]
    public async Task Resolves_Time_Zones_Through_The_Registered_Time_Zone_Provider()
    {
        var provider = TestProvider();
        var options = await Options(provider);

        await JsonSerializer.Deserialize<DateTimeZone>($"\"{TestZoneId}\"", options).Should().BeSameReferenceAs(provider[TestZoneId]);
    }

    [Test]
    public async Task Does_Not_Care_Whether_The_Time_Zone_Provider_Is_Registered_Before_Or_After_It()
    {
        var provider = TestProvider();
        var services = new ServiceCollection().AddOptions();
        new NodaTimeSystemTextJsonJoint().Register(await JointContext(), services);
        services.AddSingleton<IDateTimeZoneProvider>(provider);

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        await JsonSerializer.Deserialize<DateTimeZone>($"\"{TestZoneId}\"", options).Should().BeSameReferenceAs(provider[TestZoneId]);
    }

    [Test]
    public async Task Uses_The_Time_Zone_Source_Of_The_NodaTime_Options_Through_The_Time_Joint()
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), DovetailHostType.UnitTest, []);
        builder.AppendJoint<TimeJoint>();
        builder.AppendJoint<NodaTimeSystemTextJsonJoint>();
        builder.Set(new NodaTimeOptions { DateTimeZoneSource = TestSource(), });
        var services = await TestJoints.ApplyServicesAsync(await DovetailContext.FromAsync(builder));
        // AddOptions comes from the Foundation DefaultJoint in an application.
        services.AddOptions();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        await JsonSerializer.Deserialize<DateTimeZone>($"\"{TestZoneId}\"", options).Should().BeSameReferenceAs(provider.GetRequiredService<IDateTimeZoneProvider>()[TestZoneId]);
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Joint_That_Runs_After_The_Time_Joint()
    {
        var export = NodaTimeExports.Joints().Single(x => x.Joint is NodaTimeSystemTextJsonJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(TimeJoint));
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
        await typeof(NodaTimeSystemTextJsonJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
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
        // AddOptions comes from the Foundation DefaultJoint in an application.
        var services = new ServiceCollection().AddOptions().AddSingleton(dateTimeZoneProvider);
        new NodaTimeSystemTextJsonJoint().Register(await JointContext(), services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
    }

    private static async Task<IDovetailContext> JointContext() => await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
