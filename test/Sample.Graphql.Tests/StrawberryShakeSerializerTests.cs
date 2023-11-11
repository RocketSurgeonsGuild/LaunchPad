using Alba;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.IO;
using NodaTime;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests;

[UsesVerify]
public class StrawberryShakeSerializerTests(ITestOutputHelper testOutputHelper) : LoggerTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Roundtrip_Instant()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = clock.GetCurrentInstant();
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                Instant = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_LocalDate()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = LocalDate.FromDateOnly(DateOnly.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc()));
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                LocalDate = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_LocalTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = LocalTime.FromTimeOnly(TimeOnly.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc()));
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                LocalTime = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_LocalDateTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = LocalDateTime.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc());
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                LocalDateTime = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_OffsetDateTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = OffsetDateTime.FromDateTimeOffset(clock.GetCurrentInstant().ToDateTimeOffset());
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                OffsetDateTime = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_OffsetTime()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var clock = host.Services.GetRequiredService<IClock>();
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = new OffsetTime(LocalTime.FromTimeOnly(TimeOnly.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc())), Offset.FromHours(1));
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                OffsetTime = value
            }
        );

        await Verify(result);
    }

    // TBD
//    [Fact]
//    public async Task Should_Roundtrip_ZonedDateTime()
//    {
//        await using var host = await AlbaHost.For<Program>(new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>());
//        var clock = host.Services.GetRequiredService<IClock>();
//        var client = host.Services.GetRequiredService<IRocketClient>();
//
//
//        var value = ZonedDateTime.FromDateTimeOffset(clock.GetCurrentInstant().ToDateTimeOffset()).WithZone(DateTimeZone.ForOffset(Offset.FromHours(3)));
////        client.
//        var result = await client.GetNodaTimeTypes.ExecuteAsync(
//            new NodaTimeInputsInput()
//            {
//                ZonedDateTime = value
//            }
//        );
//
//        await Verify(result);
//    }

    [Fact]
    public async Task Should_Roundtrip_Period()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = Period.FromHours(1);
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                Period = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_Duration()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = Duration.FromHours(1);
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                Duration = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_Offset()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var client = host.Services.GetRequiredService<IRocketClient>();


        var value = Offset.FromHours(2);
//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                Offset = value
            }
        );

        await Verify(result);
    }

    [Fact]
    public async Task Should_Roundtrip_IsoDayOfWeek()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );
        var client = host.Services.GetRequiredService<IRocketClient>();

//        client.
        var result = await client.GetNodaTimeTypes.ExecuteAsync(
            new NodaTimeInputsInput()
            {
                IsoDayOfWeek = IsoDayOfWeek.Wednesday
            }
        );

        await Verify(result);
    }

    [Theory]
    [InlineData("POINT(3 4)")]
    [InlineData("LINESTRING(5 6, 7 8)")]
    [InlineData("POLYGON((9 10, 11 12, 13 14, 9 10))")]
    [InlineData("MULTIPOINT((15 16), (17 18))")]
    [InlineData("MULTILINESTRING((19 20, 21 22), (23 24, 25 26))")]
    [InlineData("MULTIPOLYGON(((27 28, 29 30, 31 32, 27 28)), ((33 34, 35 36, 37 38, 33 34)))")]
    public async Task Should_Roundtrip_Geometry(string wkt)
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
        );

        var client = host.Services.GetRequiredService<IRocketClient>();

        var reader = new WKTReader();
        var result = await client.GetGeometryTypes.ExecuteAsync(
            new()
            {
                Geometry = reader.Read(wkt)
            }
        );

        await Verify(result).UseTextForParameters(wkt);
    }


//    [Fact]
//    public async Task Should_Roundtrip_Point()
//    {
//        await using var host = await AlbaHost.For<Program>(
//            new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>()
//        );
//        
//        var client = host.Services.GetRequiredService<IRocketClient>();
//
//        var reader = new WKTReader();
//        var wkt = "POINT(3 4)";
//        var result = await client.GetGeometryTypes.ExecuteAsync(
//            new ()
//            {
//                Point = reader.Read(wkt)  
//            }
//        );
//
//        await Verify(result).UseTextForParameters(wkt);
//    }

    /*
     * geometry: "POINT(1 2)"
        point: "POINT(3 4)"
        lineString: "LINESTRING(5 6, 7 8)"
        polygon: "POLYGON((9 10, 11 12, 13 14, 9 10))"
        multiPoint: "MULTIPOINT((15 16), (17 18))"
        multiLineString: "MULTILINESTRING((19 20, 21 22), (23 24, 25 26))"
        multiPolygon: "MULTIPOLYGON(((27 28, 29 30, 31 32, 27 28)), ((33 34, 35 36, 37 38, 33 34)))"
     */
}
