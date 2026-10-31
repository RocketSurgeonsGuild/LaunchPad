using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.Foundation.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Rivets;
using Rocket.Surgery.LaunchPad.NodaTime;
using Rocket.Surgery.LaunchPad.NodaTime.Joints;

namespace NodaTime.NewtonsoftJson.Tests;

public class NodaTimeNewtonsoftJsonJointTests
{
    [Test]
    public async Task Writes_NodaTime_Values_In_The_LaunchPad_Formats()
    {
        var settings = await Settings(DateTimeZoneProviders.Tzdb);

        JsonConvert.SerializeObject(Instant.FromUtc(2020, 1, 1, 12, 12, 12), settings).ShouldBe("\"2020-01-01T12:12:12Z\"");
        JsonConvert.SerializeObject(new LocalDate(2020, 1, 1), settings).ShouldBe("\"2020-01-01\"");
    }

    [Test]
    public async Task Reads_The_Extra_Formats_That_The_LaunchPad_Converters_Accept()
    {
        var settings = await Settings(DateTimeZoneProviders.Tzdb);

        // The converter NodaTime ships rejects date time offset text for an instant, the composite one falls back to it.
        JsonConvert.DeserializeObject<Instant>("\"01/01/2020 12:12:12 +00:00\"", settings).ShouldBe(Instant.FromUtc(2020, 1, 1, 12, 12, 12));
    }

    [Test]
    public async Task Resolves_Time_Zones_Through_The_Registered_Time_Zone_Provider()
    {
        var provider = TestProvider();
        var settings = await Settings(provider);

        JsonConvert.DeserializeObject<DateTimeZone>($"\"{TestZoneId}\"", settings).ShouldBeSameAs(provider[TestZoneId]);
    }

    [Test]
    public async Task Does_Not_Care_Whether_The_Time_Zone_Provider_Is_Registered_Before_Or_After_It()
    {
        var provider = TestProvider();
        var services = new ServiceCollection().AddOptions();
        new NodaTimeNewtonsoftJsonJoint().Register(await JointContext(), services);
        services.AddSingleton<IDateTimeZoneProvider>(provider);

        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerSettings>>().Value;

        JsonConvert.DeserializeObject<DateTimeZone>($"\"{TestZoneId}\"", settings).ShouldBeSameAs(provider[TestZoneId]);
    }

    [Test]
    public async Task Works_Alongside_The_Newtonsoft_Json_Joint_And_The_Time_Joint()
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), DovetailHostType.UnitTest, []);
        builder.AppendJoint<TimeJoint>();
        builder.AppendJoint<NewtonsoftJsonJoint>();
        builder.AppendJoint<NodaTimeNewtonsoftJsonJoint>();
        builder.Set(new NodaTimeOptions { DateTimeZoneSource = TestSource(), });
        var services = await ApplyServicesAsync(await DovetailContext.FromAsync(builder));
        // AddOptions comes from the Primitives DefaultJoint in an application.
        services.AddOptions();
        using var provider = services.BuildServiceProvider();

        var settings = provider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value;

        JsonConvert.SerializeObject(DayOfWeek.Monday, settings).ShouldBe("\"monday\"");
        JsonConvert.DeserializeObject<DateTimeZone>($"\"{TestZoneId}\"", settings).ShouldBeSameAs(provider.GetRequiredService<IDateTimeZoneProvider>()[TestZoneId]);
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeNewtonsoftJsonExports.Joints().Single(x => x.Joint is NodaTimeNewtonsoftJsonJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(NodaTimeNewtonsoftJsonJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private const string TestZoneId = "UTC+03";

    private static FakeDateTimeZoneSource TestSource()
    {
        var builder = new FakeDateTimeZoneSource.Builder { VersionId = "launchpad-test", };
        builder.Add(DateTimeZone.ForOffset(Offset.FromHours(3)));
        return builder.Build();
    }

    private static DateTimeZoneCache TestProvider() => new(TestSource());

    private static async Task<JsonSerializerSettings> Settings(IDateTimeZoneProvider dateTimeZoneProvider)
    {
        // AddOptions comes from the Primitives DefaultJoint in an application.
        var services = new ServiceCollection().AddOptions().AddSingleton(dateTimeZoneProvider);
        new NodaTimeNewtonsoftJsonJoint().Register(await JointContext(), services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerSettings>>().Value;
    }

    private static async Task<IDovetailContext> JointContext() => await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));

    /// <summary>
    ///     Applies the configuration joints, then the service joints, the way a host does.
    /// </summary>
    private static async Task<IServiceCollection> ApplyServicesAsync(IDovetailContext context)
    {
        var configuration = new ConfigurationManager();
        await configuration.ApplyConfiguration(context);
        context.Set<IConfiguration>(configuration);

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        await services.ApplyService(context);
        return services;
    }
}
