using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class DateRangeTests
{
    private static readonly DateOnly January1 = new(2024, 1, 1);
    private static readonly DateOnly January31 = new(2024, 1, 31);

    [Test]
    public void Accepts_A_Start_Before_Or_On_The_End()
    {
        DateRange.TryFrom(January1, January31, out var range).ShouldBeTrue();
        range.ShouldBe(DateRange.From(January1, January31));
        range!.Start.ShouldBe(January1);
        range.End.ShouldBe(January31);
        DateRange.From(January1, January1).End.ShouldBe(January1);
    }

    [Test]
    public void Rejects_A_Start_After_The_End()
    {
        DateRange.TryFrom(January31, January1, out var range).ShouldBeFalse();
        range.ShouldBeNull();
        Should.Throw<ArgumentException>(() => DateRange.From(January31, January1));
        Should.Throw<ArgumentException>(() => new DateRange(January31, January1));
    }

    [Test]
    public void Contains_Both_Ends()
    {
        var range = DateRange.From(January1, January31);
        range.Contains(January1).ShouldBeTrue();
        range.Contains(January31).ShouldBeTrue();
        range.Contains(new(2024, 2, 1)).ShouldBeFalse();
        range.Contains(new(2023, 12, 31)).ShouldBeFalse();
    }

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(DateRange.From(January1, January31), """{"start":"2024-01-01","end":"2024-01-31"}""");
        FailsToRead<DateRange>("""{"start":"2024-01-31","end":"2024-01-01"}""");
    }
}

public class TimeRangeTests
{
    private static readonly TimeOnly Nine = new(9, 0);
    private static readonly TimeOnly FiveThirty = new(17, 30);

    [Test]
    public void Accepts_A_Start_Before_Or_On_The_End()
    {
        TimeRange.TryFrom(Nine, FiveThirty, out var range).ShouldBeTrue();
        range.ShouldBe(TimeRange.From(Nine, FiveThirty));
        range!.Duration.ShouldBe(TimeSpan.FromHours(8.5));
        TimeRange.From(Nine, Nine).Duration.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void Rejects_A_Range_That_Runs_Past_Midnight()
    {
        TimeRange.TryFrom(new(22, 0), new(2, 0), out var range).ShouldBeFalse();
        range.ShouldBeNull();
        Should.Throw<ArgumentException>(() => TimeRange.From(new(22, 0), new(2, 0)));
    }

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(TimeRange.From(Nine, FiveThirty), """{"start":"09:00:00","end":"17:30:00"}""");
        FailsToRead<TimeRange>("""{"start":"17:30:00","end":"09:00:00"}""");
    }
}

public class MoneyTests
{
    private static readonly CurrencyCode Usd = CurrencyCode.From("USD");

    [Test]
    [Arguments(0d)]
    [Arguments(12.5d)]
    [Arguments(-12.5d)]
    [Arguments(0.0001d)]
    public void Accepts_Any_Amount(double amount)
    {
        Money.TryFrom((decimal)amount, Usd, out var money).ShouldBeTrue();
        money.ShouldBe(Money.From((decimal)amount, Usd));
        money!.Amount.ShouldBe((decimal)amount);
        money.Currency.ShouldBe(Usd);
    }

    [Test]
    public void Rejects_No_Currency()
    {
        Money.TryFrom(1m, null, out var money).ShouldBeFalse();
        money.ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => Money.From(1m, null!));
    }

    [Test]
    public void Compares_Amount_And_Currency()
    {
        Money.From(1m, Usd).ShouldBe(Money.From(1m, CurrencyCode.From("USD")));
        Money.From(1m, Usd).ShouldNotBe(Money.From(1m, CurrencyCode.From("EUR")));
        Money.From(1m, Usd).ToString().ShouldBe("1 USD");
    }

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Money.From(12.5m, Usd), """{"amount":12.5,"currency":"USD"}""");
        FailsToRead<Money>("""{"amount":12.5,"currency":"usd"}""");
        FailsToRead<Money>("""{"amount":12.5}""");
    }
}
