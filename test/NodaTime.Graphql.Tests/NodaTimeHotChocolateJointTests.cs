using System.Reflection;
using System.Text.Json;

using HotChocolate;
using HotChocolate.Execution;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Rivets;

namespace NodaTime.Graphql.Tests;

public class NodaTimeHotChocolateJointTests
{
    [Test]
    [Arguments("dateTime", "2020-01-02T03:04:05.5+01:00")]
    [Arguments("duration", "P1DT2H3M4.5S")]
    [Arguments("localDate", "2020-01-02")]
    [Arguments("localDateTime", "2020-01-02T03:04:05.5")]
    [Arguments("localTime", "03:04:05.5")]
    public async Task Round_Trips_Each_Supported_NodaTime_Type(string field, string value)
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync($$"""{ {{field}}(value: "{{value}}") }""") ).ExpectOperationResult();

        await string.Join("; ", result.Errors?.Select(x => x.Message) ?? []).Should().BeEmpty();
        await result.ToJson().Should().Contain(value);
    }

    [Test]
    [Arguments("2020-01-02T03:04:05.5Z", "2020-01-02T03:04:05.5Z", "2020-01-02T04:04:05.5Z")]
    [Arguments("2020-01-02T03:04:05.5+01:00", "2020-01-02T02:04:05.5Z", "2020-01-02T03:04:05.5Z")]
    public async Task Round_Trips_An_Instant_As_A_DateTime_In_Utc(string value, string expected, string anHourLater)
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync(
            $$"""{ instant(value: "{{value}}") nullableInstant(value: "{{value}}") nextInstant(value: "{{value}}") }"""
        ) ).ExpectOperationResult();

        await string.Join("; ", result.Errors?.Select(x => x.Message) ?? []).Should().BeEmpty();
        var data = JsonElement.Parse(result.ToJson()).GetProperty("data");
        await data.GetProperty("instant").GetString().Should().BeEqualTo(expected);
        await data.GetProperty("nullableInstant").GetString().Should().BeEqualTo(expected);
        // the resolver received an Instant, not the text
        await data.GetProperty("nextInstant").GetString().Should().BeEqualTo(anHourLater);
    }

    [Test]
    public async Task Binds_An_Instant_To_The_DateTime_Scalar()
    {
        var schema = ( await Executor() ).Schema.ToString();

        await schema.Should().Contain("instant(value: DateTime!): DateTime!");
        await schema.Should().Contain("nullableInstant(value: DateTime): DateTime");
        await schema.Should().NotContain("scalar Instant");
    }

    [Test]
    public async Task Filters_An_Instant_With_The_DateTime_Scalar()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync(
            """
            {
              records(where: { instant: { gt: "2020-01-01T12:00:00+01:00" } }) { instant }
            }
            """
        ) ).ExpectOperationResult();

        await string.Join("; ", result.Errors?.Select(x => x.Message) ?? []).Should().BeEmpty();
        var records = JsonElement.Parse(result.ToJson()).GetProperty("data").GetProperty("records");
        await records.EnumerateArray().Select(x => x.GetProperty("instant").GetString()).Should().BeEquivalentTo(["2020-01-02T00:00:00Z"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Adds_HotChocolates_NodaTime_Scalars_With_The_DateTime_Name_For_OffsetDateTime()
    {
        var schema = ( await Executor() ).Schema.ToString();

        await schema.Should().Contain("scalar DateTime");
        await schema.Should().Contain("scalar Duration");
        await schema.Should().Contain("scalar LocalDate");
        await schema.Should().Contain("scalar LocalDateTime");
        await schema.Should().Contain("scalar LocalTime");
        await schema.Should().NotContain("scalar OffsetDateTime");
    }

    [Test]
    public async Task Binds_The_NodaTime_Types_To_Comparable_Filters()
    {
        var schema = ( await Executor() ).Schema.ToString();

        // the filter inputs are named for the runtime types, as they were before HotChocolate 16
        await schema.Should().Contain("dateTime: ComparableOffsetDateTimeOperationFilterInput");
        await schema.Should().Contain("duration: ComparableDurationOperationFilterInput");
        await schema.Should().Contain("localDate: ComparableLocalDateOperationFilterInput");
        await schema.Should().Contain("instant: ComparableInstantOperationFilterInput");
    }

    [Test]
    public async Task Filters_The_Bcl_Date_Types_With_The_NodaTime_Scalars()
    {
        // HotChocolate.Data's own filters for these types are built on the scalars that the NodaTime ones replace, by name
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync(
            """
            {
              records(
                where: {
                  offset: { gt: "2020-01-01T00:00:00Z" }
                  local: { lt: "2020-01-01T00:00:00" }
                  date: { eq: "2020-01-01" }
                  time: { neq: "10:00:00" }
                }
              ) { localDate }
            }
            """
        ) ).ExpectOperationResult();

        await string.Join("; ", result.Errors?.Select(x => x.Message) ?? []).Should().BeEmpty();
    }

    [Test]
    public async Task Is_Exported_As_An_Application_Joint_Without_Ordering()
    {
        var export = NodaTimeHotChocolateExports.Joints().Single(x => x.Joint is NodaTimeHotChocolateJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
        await typeof(NodaTimeHotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    private static async Task<IRequestExecutor> Executor()
    {
        var serviceCollection = new ServiceCollection();
        var dovetailContext = await JointContext.Create();
        dovetailContext
           .GetOrAdd(() => serviceCollection.AddGraphQL())
           .AddFiltering()
           .AddQueryType<NodaTimeQuery>();
        new NodaTimeHotChocolateJoint().Register(dovetailContext, serviceCollection);
        return await serviceCollection.BuildServiceProvider().GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
    }
}

public class NodaTimeQuery
{
    public OffsetDateTime DateTime(OffsetDateTime value) => value;
    public Duration Duration(Duration value) => value;
    public LocalDate LocalDate(LocalDate value) => value;
    public LocalDateTime LocalDateTime(LocalDateTime value) => value;
    public LocalTime LocalTime(LocalTime value) => value;
    public Instant Instant(Instant value) => value;
    public Instant? NullableInstant(Instant? value) => value;
    public Instant NextInstant(Instant value) => value.Plus(NodaTime.Duration.FromHours(1));

    [HotChocolate.Data.UseFiltering]
    public IQueryable<NodaTimeRecord> Records() =>
        new[]
        {
            Record(NodaTime.Instant.FromUtc(2020, 1, 1, 0, 0)),
            Record(NodaTime.Instant.FromUtc(2020, 1, 2, 0, 0)),
        }.AsQueryable();

    private static NodaTimeRecord Record(Instant instant) =>
        new(instant.WithOffset(Offset.Zero), NodaTime.Duration.Zero, default, default, default, default, default, instant);
}

public record NodaTimeRecord(
    OffsetDateTime DateTime,
    Duration Duration,
    LocalDate LocalDate,
    DateTimeOffset Offset,
    DateTime Local,
    DateOnly Date,
    TimeOnly Time,
    Instant Instant
);
