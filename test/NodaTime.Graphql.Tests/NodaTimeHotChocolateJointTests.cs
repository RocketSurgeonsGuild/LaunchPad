using System.Reflection;

using HotChocolate.Execution;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using NodaTime.Testing.TimeZones;
using NodaTime.Text;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.NodaTime;
using Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Rivets;
using Rocket.Surgery.LaunchPad.NodaTime.Joints;

namespace NodaTime.Graphql.Tests;

public class NodaTimeHotChocolateJointTests
{
    [Test]
    public async Task Adds_The_NodaTime_Types_Using_The_Default_Time_Zone_Source()
    {
        var executor = await Executor();

        ( await ZoneOf(executor, ZonedDateTime("America/New_York", DateTimeZoneProviders.Tzdb)) ).ShouldBe("America/New_York");
    }

    [Test]
    public async Task Reads_The_Time_Zone_Source_From_The_Context()
    {
        var provider = new DateTimeZoneCache(TestZoneSource());
        var executor = await Executor(context: context => context.Set(new NodaTimeOptions { DateTimeZoneSource = TestZoneSource() }));

        ( await ZoneOf(executor, ZonedDateTime("Fake/Zone", provider)) ).ShouldBe("Fake/Zone");
    }

    [Test]
    public async Task Does_Not_Know_The_Test_Time_Zone_Without_The_Context_Option()
    {
        var provider = new DateTimeZoneCache(TestZoneSource());
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync($$"""{ zoneOf(value: "{{ZonedDateTime("Fake/Zone", provider)}}") }""") ).ExpectOperationResult();

        result.Errors.ShouldNotBeNull().ShouldNotBeEmpty();
    }

    [Test]
    public async Task Prefers_The_Time_Zone_Provider_Instance_Registered_In_The_Services()
    {
        var provider = new DateTimeZoneCache(TestZoneSource());
        var executor = await Executor(services => services.AddSingleton<IDateTimeZoneProvider>(provider));

        ( await ZoneOf(executor, ZonedDateTime("Fake/Zone", provider)) ).ShouldBe("Fake/Zone");
    }

    [Test]
    public void Is_Exported_As_An_Application_Joint_That_Runs_After_The_Time_Joint()
    {
        var export = NodaTimeHotChocolateExports.Joints().Single(x => x.Joint is NodaTimeHotChocolateJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(TimeJoint))
        );
        export.Category.ToString().ShouldBe(DovetailCategory.Application);
        typeof(NodaTimeHotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    private static IDateTimeZoneSource TestZoneSource()
    {
        var builder = new FakeDateTimeZoneSource.Builder { VersionId = "test" };
        builder.Add(new SingleTransitionDateTimeZone(Instant.FromUtc(2020, 1, 1, 0, 0), Offset.FromHours(1), Offset.FromHours(2), "Fake/Zone"));
        return builder.Build();
    }

    // The text the ZonedDateTime scalar accepts: the pattern the Joint builds from the time zone provider.
    private static string ZonedDateTime(string zoneId, IDateTimeZoneProvider provider) =>
        ZonedDateTimePattern
           .CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider)
           .Format(new LocalDateTime(2020, 1, 1, 10, 0).InZoneStrictly(provider[zoneId]));

    private static async Task<string?> ZoneOf(IRequestExecutor executor, string value)
    {
        var result = ( await executor.ExecuteAsync($$"""{ zoneOf(value: "{{value}}") }""") ).ExpectOperationResult();
        string.Join("; ", result.Errors?.Select(x => x.Message) ?? []).ShouldBeEmpty();
        return (string?)result.Data!["zoneOf"];
    }

    private static async Task<IRequestExecutor> Executor(Action<IServiceCollection>? services = null, Action<IDovetailContext>? context = null)
    {
        var serviceCollection = new ServiceCollection();
        services?.Invoke(serviceCollection);
        var dovetailContext = await JointContext.Create(context);
        dovetailContext.GetOrAdd(() => serviceCollection.AddGraphQL()).AddQueryType<ZoneQuery>();
        new NodaTimeHotChocolateJoint().Register(dovetailContext, serviceCollection);
        return await serviceCollection.BuildServiceProvider().GetRequiredService<IRequestExecutorResolver>().GetRequestExecutorAsync();
    }
}

public class ZoneQuery
{
    public string ZoneOf(ZonedDateTime value) => value.Zone.Id;
}
