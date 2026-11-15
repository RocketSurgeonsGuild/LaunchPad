using Alba;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NodaTime;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests;

public class StrawberryShakeSerializerTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_Roundtrip_LocalDate()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var timeProvider = host.Services.GetRequiredService<TimeProvider>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = LocalDate.FromDateOnly(DateOnly.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc()));
        //        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new()
            {
                LocalDate = value,
            },
            cancellationToken: TestContext.CancellationToken
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Roundtrip_LocalTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = LocalTime.FromTimeOnly(TimeOnly.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc()));
        //        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new()
            {
                LocalTime = value,
            },
            cancellationToken: TestContext.CancellationToken
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Roundtrip_LocalDateTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = LocalDateTime.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc());
        //        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new()
            {
                LocalDateTime = value,
            },
            cancellationToken: TestContext.CancellationToken
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Roundtrip_OffsetDateTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = OffsetDateTime.FromDateTimeOffset(clock.GetCurrentInstant().ToDateTimeOffset());
        //        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new()
            {
                OffsetDateTime = value,
            },
            cancellationToken: TestContext.CancellationToken
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Roundtrip_Duration()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = Duration.FromHours(1);
        //        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new()
            {
                Duration = value,
            },
            cancellationToken: TestContext.CancellationToken
        );

        await Verify(result);
    }

    [Test]
    [Arguments("POINT(3 4)")]
    [Arguments("LINESTRING(5 6, 7 8)")]
    [Arguments("POLYGON((9 10, 11 12, 13 14, 9 10))")]
    [Arguments("MULTIPOINT((15 16), (17 18))")]
    [Arguments("MULTILINESTRING((19 20, 21 22), (23 24, 25 26))")]
    [Arguments("MULTIPOLYGON(((27 28, 29 30, 31 32, 27 28)), ((33 34, 35 36, 37 38, 33 34)))")]
    [Arguments("GEOMETRYCOLLECTION(POINT(39 40), LINESTRING(41 42, 43 44))")]
    public async Task Should_Roundtrip_Geometry(string wkt)
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );

        var client = host.Services.GetRequiredService<IRocketClient>();

        var reader = new WKTReader();
        var result = await client.GetGeometryTypes.ExecuteAsync(
            new()
            {
                Geometry = reader.Read(wkt),
            },
            cancellationToken: TestContext.CancellationToken
        );

        await Verify(result).UseTextForParameters(wkt);
    }


    [Test]
    public async Task Should_Roundtrip_Point()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );

        var client = host.Services.GetRequiredService<IRocketClient>();

        var point = new Point(3, 4);
        var result = await client.GetGeometryTypes.ExecuteAsync(
            new()
            {
                Geometry = point,
            },
            cancellationToken: TestContext.CancellationToken
        );

        await result.Errors.Should().BeEmpty();
        var data = result.Data!.GeometryTest;
        await data.Geometry.Should().BeEqualTo(point);
        await data.Point!.Type.Should().BeEqualTo(GeoJSONGeometryType.Point);
        await data.Point.Coordinates.Should().BeEqualTo(new Coordinate(3, 4));
        await data.LineString.Should().BeNull();
        await data.GeometryCollection.Should().BeNull();
    }
}
