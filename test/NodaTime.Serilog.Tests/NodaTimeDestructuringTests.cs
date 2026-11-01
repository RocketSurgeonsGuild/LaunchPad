using NodaTime.Testing;

using Rocket.Surgery.LaunchPad.Serilog;

using Serilog.Context;
using Serilog.Events;

namespace NodaTime.Serilog.Tests;

public class NodaTimeDestructuringTests : LoggerTest<TestRecord>
{
    [Test]
    public async Task Should_Destructure_NodaTime_Instant()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", _clock.GetCurrentInstant());
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_LocalDateTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", LocalDateTime.FromDateTime(_clock.GetCurrentInstant().ToDateTimeUtc()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_LocalDate()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", LocalDate.FromDateTime(_clock.GetCurrentInstant().ToDateTimeUtc()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_LocalTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", LocalTime.FromHoursSinceMidnight(4));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_OffsetDateTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", OffsetDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_OffsetDate()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", OffsetDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()).ToOffsetDate());
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_OffsetTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", OffsetDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()).ToOffsetTime());
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_ZonedDateTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", ZonedDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_DateTimeZone()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", DateTimeZoneProviders.Tzdb["America/New_York"]);
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_Duration()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", Duration.FromDays(1));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_Period()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", Period.FromDays(1));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NodaTime_Interval()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", new Interval(_clock.GetCurrentInstant(), _clock.GetCurrentInstant()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    public NodaTimeDestructuringTests() : base(
        new(
            LogEventLevel.Information,
            configureLogger: (_, configuration) => configuration.Destructure.NodaTimeTypes()
        )
    )
    {
        LogContext.PushProperty("SourceContext", nameof(NodaTimeDestructuringTests));
        _clock = new(Instant.FromUtc(2022, 1, 1, 4, 4, 4));
    }

    private readonly FakeClock _clock;
}
