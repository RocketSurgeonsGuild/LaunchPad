using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;
using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Rivets;

using ISerializer = StrawberryShake.Serialization.ISerializer;

namespace NodaTime.Graphql.Tests;

public class NodaTimeStrawberryShakeJointTests
{
    [Test]
    public async Task Registers_A_Serializer_For_Every_NodaTime_Scalar()
    {
        var serializers = await Serializers(new ServiceCollection());

        serializers.Select(x => (x.GetType(), x.TypeName)).ShouldBe(
            [
                (typeof(InstantSerializer), "Instant"),
                (typeof(LocalDateSerializer), "LocalDate"),
                (typeof(LocalTimeSerializer), "LocalTime"),
                (typeof(LocalDateTimeSerializer), "LocalDateTime"),
                (typeof(OffsetDateTimeSerializer), "OffsetDateTime"),
                (typeof(OffsetDateSerializer), "OffsetDate"),
                (typeof(OffsetTimeSerializer), "OffsetTime"),
                (typeof(PeriodSerializer), "Period"),
                (typeof(DurationSerializer), "Duration"),
                (typeof(ZonedDateTimeSerializer), "ZonedDateTime"),
                (typeof(OffsetSerializer), "Offset"),
                (typeof(IsoDayOfWeekSerializer), "IsoDayOfWeek"),
                (typeof(DateTimeZoneSerializer), "DateTimeZone"),
            ],
            ignoreOrder: true
        );
    }

    [Test]
    public async Task Registers_The_Serializers_As_Singletons()
    {
        var services = new ServiceCollection();
        new NodaTimeStrawberryShakeJoint().Register(await JointContext.Create(), services);

        services.Where(x => x.ServiceType == typeof(ISerializer)).ShouldAllBe(x => x.Lifetime == ServiceLifetime.Singleton);
    }

    [Test]
    public async Task The_Time_Zone_Serializer_Uses_The_Registered_Time_Zone_Provider()
    {
        var provider = new DateTimeZoneCache(TestZoneSource());
        var services = new ServiceCollection().AddSingleton<IDateTimeZoneProvider>(provider);

        var serializer = ( await Serializers(services) ).OfType<DateTimeZoneSerializer>().Single();

        serializer.Parse("Fake/Zone").ShouldBe(provider["Fake/Zone"]);
    }

    [Test]
    public async Task Resolves_Every_Serializer_With_The_Time_Zone_Provider_From_The_Time_Joint()
    {
        // the imported joints, as an application gets them: the TimeJoint of Rocket.Surgery.LaunchPad.NodaTime registers the provider
        var context = await DovetailContext.FromAsync(Imports.Joints());
        var services = new ServiceCollection();
        await services.ApplyService(context);

        var provider = services.BuildServiceProvider();
        var serializers = provider.GetServices<ISerializer>().ToList();

        serializers.Count.ShouldBe(13);
        serializers.OfType<DateTimeZoneSerializer>().Single().Parse("America/New_York")
                   .ShouldBeSameAs(provider.GetRequiredService<IDateTimeZoneProvider>()["America/New_York"]);
    }

    [Test]
    public async Task The_Offset_Date_Serializer_Uses_The_Pattern_The_Server_Scalar_Accepts_First()
    {
        var serializer = ( await Serializers(new ServiceCollection()) ).OfType<OffsetDateSerializer>().Single();
        var value = new OffsetDate(new LocalDate(2020, 1, 2), Offset.FromHours(1));

        // OffsetDatePattern.GeneralIso, the first pattern of the OffsetDate type that AddNodaTime adds to the HotChocolate schema
        serializer.Parse("2020-01-02+01").ShouldBe(value);
        serializer.Format(value).ShouldBe("2020-01-02+01");
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeStrawberryShakeExports.Joints().Single(x => x.Joint is NodaTimeStrawberryShakeJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(NodaTimeStrawberryShakeJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    // ZonedDateTimeSerializer and DateTimeZoneSerializer need the IDateTimeZoneProvider that the TimeJoint registers in an application
    private static async Task<List<ISerializer>> Serializers(IServiceCollection services)
    {
        services.TryAddSingleton(DateTimeZoneProviders.Tzdb);
        new NodaTimeStrawberryShakeJoint().Register(await JointContext.Create(), services);
        return [.. services.BuildServiceProvider().GetServices<ISerializer>()];
    }

    private static IDateTimeZoneSource TestZoneSource()
    {
        var builder = new FakeDateTimeZoneSource.Builder { VersionId = "test" };
        builder.Add(new SingleTransitionDateTimeZone(Instant.FromUtc(2020, 1, 1, 0, 0), Offset.FromHours(1), Offset.FromHours(2), "Fake/Zone"));
        return builder.Build();
    }
}
