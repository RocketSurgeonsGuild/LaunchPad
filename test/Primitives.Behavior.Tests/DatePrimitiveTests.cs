using Microsoft.Extensions.Time.Testing;

using NodaTime;
using NodaTime.Testing;

using Rocket.Surgery.LaunchPad.Primitives;

using static Primitives.Behavior.Tests.PrimitiveAssert;

namespace Primitives.Behavior.Tests;

/// <summary>
///     "Now" comes from a TimeProvider (or for NodaTime an IClock) passed to Create and TryCreate, so every check here runs at a fixed time:
///     Monday 23 November 2026, 10:30 UTC.
/// </summary>
public class DatePrimitiveTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 23, 10, 30, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);
    private static FakeTimeProvider Time => new(Now);

    [Test]
    public async Task Should_Accept_Only_The_Past_When_Past_Only()
    {
        await PastDay.TryCreate(Today.AddDays(-1), Time, out _, out _).Should().BeTrue();
        // today is not in the future: it counts as the past
        await PastDay.TryCreate(Today, Time, out _, out _).Should().BeTrue();
        await PastDay.TryCreate(Today.AddDays(1), Time, out var future, out var error).Should().BeFalse();
        await future.IsDefault.Should().BeTrue();
        await error.Code.Should().BeEqualTo("TEMPORAL");
        await error.Message.Should().BeEqualTo("PastDay must be in the past.");

        var exception = await Throws<DomainPrimitiveValidationException>(() => PastDay.Create(Today.AddDays(1), Time));
        await exception!.Error.Code.Should().BeEqualTo("TEMPORAL");
    }

    [Test]
    public async Task Should_Accept_Only_The_Future_When_Future_Only()
    {
        await FutureDay.TryCreate(Today.AddDays(1), Time, out var tomorrow, out _).Should().BeTrue();
        await tomorrow!.Value.Should().BeEqualTo(Today.AddDays(1));
        // today is not in the future
        await FutureDay.TryCreate(Today, Time, out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("TEMPORAL");
        await ShiftStartTime.TryCreate(new TimeOnly(10, 31), Time, out _, out _).Should().BeTrue();
        await ShiftStartTime.TryCreate(new TimeOnly(10, 30), Time, out _, out _).Should().BeFalse();
    }

    [Test]
    public async Task Should_Read_Now_From_The_System_Without_A_Time_Provider()
    {
        await Accepts<PastDay, DateOnly>(DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1));
        await Rejects<PastDay, DateOnly>(DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1), "TEMPORAL");
        await Accepts<GlobalTimestamp, DateTimeOffset>(DateTimeOffset.UtcNow.AddMinutes(-1));
        await Rejects<GlobalTimestamp, DateTimeOffset>(DateTimeOffset.UtcNow.AddMinutes(1), "TEMPORAL");
    }

    [Test]
    public async Task Should_Move_With_The_Time_Provider()
    {
        var time = Time;
        var moment = Now.AddHours(1);
        await GlobalTimestamp.TryCreate(moment, time, out _, out _).Should().BeFalse();
        time.Advance(TimeSpan.FromHours(2));
        await GlobalTimestamp.TryCreate(moment, time, out var created, out _).Should().BeTrue();
        await created.Value.Should().BeEqualTo(moment);
    }

    [Test]
    public async Task Should_Check_A_Range_Of_Times()
    {
        await Accepts<OfficeHour, TimeOnly>(new TimeOnly(9, 0));
        await Accepts<OfficeHour, TimeOnly>(new TimeOnly(17, 0));
        var error = await Rejects<OfficeHour, TimeOnly>(new TimeOnly(8, 59), "RANGE");
        await error.Message.Should().StartWith("OfficeHour must be at least 09:00.");
        await Accepts<CenturyDay, DateOnly>(new DateOnly(2000, 1, 1));
        await Rejects<CenturyDay, DateOnly>(new DateOnly(2100, 1, 1), "CENTURY");
    }

    [Test]
    public async Task Should_Validate_A_Birth_Date()
    {
        await CustomerBirthDate.TryCreate(new DateOnly(1990, 5, 1), Time, out var born, out _).Should().BeTrue();
        await born.GetAge(Time).Should().BeEqualTo(36);
        // a birthday later in the year hasn't happened yet
        await CustomerBirthDate.Create(new DateOnly(2000, 12, 1), Time).GetAge(Time).Should().BeEqualTo(25);
        await CustomerBirthDate.TryCreate(Today.AddDays(1), Time, out _, out var future).Should().BeFalse();
        await future.Code.Should().BeEqualTo("TEMPORAL");

        // at most MaxAge (120) years ago, to the day
        await CustomerBirthDate.TryCreate(Today.AddYears(-120), Time, out _, out _).Should().BeTrue();
        await CustomerBirthDate.TryCreate(Today.AddYears(-120).AddDays(-1), Time, out _, out var old).Should().BeFalse();
        await old.Message.Should().BeEqualTo("CustomerBirthDate must be at most 120 years ago.");
    }

    [Test]
    public async Task Should_Count_The_Age_Of_A_Leap_Day()
    {
        var born = CustomerBirthDate.Create(new DateOnly(2000, 2, 29), Time);

        await born.GetAge(new FakeTimeProvider(new DateTimeOffset(2025, 2, 28, 0, 0, 0, TimeSpan.Zero))).Should().BeEqualTo(24);
        await born.GetAge(new FakeTimeProvider(new DateTimeOffset(2025, 3, 1, 0, 0, 0, TimeSpan.Zero))).Should().BeEqualTo(25);
    }

    [Test]
    public async Task Should_Validate_An_Expiration_Date()
    {
        var expires = CreditCardExpiration.Create(Today.AddDays(10), Time);

        await expires.IsExpired(Time).Should().BeFalse();
        await expires.DaysUntilExpiration(Time).Should().BeEqualTo(10);
        var later = new FakeTimeProvider(Now.AddDays(12));
        await expires.IsExpired(later).Should().BeTrue();
        await expires.DaysUntilExpiration(later).Should().BeEqualTo(-2);
        await CreditCardExpiration.TryCreate(Today, Time, out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("TEMPORAL");
    }

    [Test]
    public async Task Should_Accept_Only_Weekdays_As_A_Business_Date()
    {
        await Accepts<WorkDay, DateOnly>(new DateOnly(2026, 11, 23));
        await Accepts<WorkDay, DateOnly>(new DateOnly(2026, 11, 27));
        var error = await Rejects<WorkDay, DateOnly>(new DateOnly(2026, 11, 28), "TEMPORAL");
        await error.Message.Should().BeEqualTo("WorkDay must be a weekday.");
        await Rejects<WorkDay, DateOnly>(new DateOnly(2026, 11, 29), "TEMPORAL");
        await Accepts<AnyDay, DateOnly>(new DateOnly(2026, 11, 29));
    }

    [Test]
    public async Task Should_Have_No_Value_When_Default()
    {
        await default(WorkShiftTime).IsDefault.Should().BeTrue();
        await WorkShiftTime.Create(TimeOnly.MinValue, new TimeOnly(8, 0)).IsDefault.Should().BeFalse();
        await PastDay.Create(Today.AddDays(-1), Time).IsDefault.Should().BeFalse();
        await default(PastDay).IsDefault.Should().BeTrue();
    }
}

public class NodaTimePrimitiveTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 23, 10, 30);
    private static FakeClock Clock => new(Now);

    [Test]
    public async Task Should_Read_Now_From_The_Clock()
    {
        await HappenedAt.TryCreate(Now - Duration.FromMinutes(1), Clock, out _, out _).Should().BeTrue();
        await HappenedAt.TryCreate(Now + Duration.FromMinutes(1), Clock, out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("TEMPORAL");

        var clock = Clock;
        var later = Now + Duration.FromHours(1);
        await HappenedAt.TryCreate(later, clock, out _, out _).Should().BeFalse();
        clock.Advance(Duration.FromHours(2));
        await HappenedAt.TryCreate(later, clock, out _, out _).Should().BeTrue();
    }

    [Test]
    public async Task Should_Order_A_Zoned_Time_By_Its_Instant()
    {
        var zone = DateTimeZoneProviders.Tzdb["America/Toronto"];
        var soon = ( Now + Duration.FromHours(1) ).InZone(zone);
        var past = ( Now - Duration.FromHours(1) ).InZone(zone);

        await Appointment.TryCreate(soon, Clock, out var appointment, out _).Should().BeTrue();
        await appointment!.Value.Should().BeEqualTo(soon);
        await Appointment.TryCreate(past, Clock, out _, out var error).Should().BeFalse();
        await error.Message.Should().BeEqualTo("Appointment must be in the future.");
    }

    [Test]
    public async Task Should_Check_A_Range_Of_Dates()
    {
        await Accepts<ReportDay, LocalDate>(new LocalDate(2000, 1, 1));
        await Accepts<ReportDay, LocalDate>(new LocalDate(2100, 12, 31));
        await Rejects<ReportDay, LocalDate>(new LocalDate(1999, 12, 31), "RANGE");
        await Rejects<ReportDay, LocalDate>(new LocalDate(2101, 1, 1), "RANGE");
    }

    [Test]
    public async Task Should_Validate_The_NodaTime_Date_Kinds()
    {
        var born = NodaBirthDate.Create(new LocalDate(1990, 11, 24), Clock);
        await born.GetAge(Clock).Should().BeEqualTo(35);
        await NodaBirthDate.TryCreate(new LocalDate(2026, 11, 24), Clock, out _, out _).Should().BeFalse();
        await NodaBirthDate.TryCreate(new LocalDate(1876, 11, 22), Clock, out _, out _).Should().BeFalse();

        var expires = NodaExpiration.Create(new LocalDate(2026, 12, 3), Clock);
        await expires.DaysUntilExpiration(Clock).Should().BeEqualTo(10);
        await expires.IsExpired(Clock).Should().BeFalse();
        await expires.IsExpired(new FakeClock(Now + Duration.FromDays(10))).Should().BeTrue();

        await Accepts<NodaWorkDay, LocalDate>(new LocalDate(2026, 11, 23));
        await Rejects<NodaWorkDay, LocalDate>(new LocalDate(2026, 11, 22), "TEMPORAL");

        await BillingMonth.TryCreate(new YearMonth(2026, 11), Clock, out _, out _).Should().BeTrue();
        await BillingMonth.TryCreate(new YearMonth(2026, 12), Clock, out _, out _).Should().BeFalse();
    }

    [Test]
    public async Task Should_Hold_The_NodaTime_Values_Without_A_Time_Rule()
    {
        await Accepts<RecordedAt, Instant>(Now);
        await Accepts<OffsetMoment, OffsetDateTime>(Now.WithOffset(Offset.FromHours(-5)));
        await Accepts<AlarmTime, LocalTime>(new LocalTime(6, 45));
        await Accepts<Wallclock, LocalDateTime>(new LocalDateTime(2026, 11, 23, 10, 30));
        await Accepts<Elapsed, Duration>(Duration.FromMinutes(90));
        await Accepts<Term, Period>(Period.FromMonths(6));
        await Accepts<Anniversary, AnnualDate>(new AnnualDate(2, 29));
        await Accepts<UtcOffset, Offset>(Offset.FromHoursAndMinutes(5, 30));
    }

    [Test]
    public async Task Should_Write_And_Parse_Its_Iso_Text()
    {
        await RecordedAt.Create(Now).ToString().Should().BeEqualTo("2026-11-23T10:30:00Z");
        await RecordedAt.Parse("2026-11-23T10:30:00Z", null).Value.Should().BeEqualTo(Now);
        await ReportDay.Parse("2050-06-01", null).Value.Should().BeEqualTo(new LocalDate(2050, 6, 1));
        await Term.Create(Period.FromDays(3)).ToString().Should().BeEqualTo("P3D");
        await Elapsed.Parse("1:30:00", null).Value.Should().BeEqualTo(Duration.FromMinutes(90));
        await Anniversary.Create(new AnnualDate(2, 29)).ToString().Should().BeEqualTo("02-29");
        await UtcOffset.Parse("+05:30", null).Value.Should().BeEqualTo(Offset.FromHoursAndMinutes(5, 30));
        await ReportDay.TryParse("not a date", null, out _).Should().BeFalse();
        // parsed, then checked by the rules
        await ReportDay.TryParse("1999-01-01", null, out _).Should().BeFalse();
    }
}
