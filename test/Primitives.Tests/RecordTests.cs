using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class DateRangeTests
{
    private static readonly DateOnly January1 = new(2024, 1, 1);
    private static readonly DateOnly January31 = new(2024, 1, 31);

    [Test]
    public async Task Accepts_A_Start_Before_Or_On_The_End()
    {
        await DateRange.TryFrom(January1, January31, out var range).Should().BeTrue();
        await range.Should().BeEqualTo(DateRange.From(January1, January31));
        await range!.Start.Should().BeEqualTo(January1);
        await range.End.Should().BeEqualTo(January31);
        await DateRange.From(January1, January1).End.Should().BeEqualTo(January1);
    }

    [Test]
    public async Task Rejects_A_Start_After_The_End()
    {
        await DateRange.TryFrom(January31, January1, out var range).Should().BeFalse();
        await range.Should().BeNull();
        await ( (Action)( () => DateRange.From(January31, January1) ) ).Should().Throw<ArgumentException>();
        await ( (Action)( () => new DateRange(January31, January1) ) ).Should().Throw<ArgumentException>();
    }

    [Test]
    public async Task Contains_Both_Ends()
    {
        var range = DateRange.From(January1, January31);
        await range.Contains(January1).Should().BeTrue();
        await range.Contains(January31).Should().BeTrue();
        await range.Contains(new(2024, 2, 1)).Should().BeFalse();
        await range.Contains(new(2023, 12, 31)).Should().BeFalse();
    }

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(DateRange.From(January1, January31), """{"start":"2024-01-01","end":"2024-01-31"}""");
        await FailsToRead<DateRange>("""{"start":"2024-01-31","end":"2024-01-01"}""");
    }
}

public class TimeRangeTests
{
    private static readonly TimeOnly Nine = new(9, 0);
    private static readonly TimeOnly FiveThirty = new(17, 30);

    [Test]
    public async Task Accepts_A_Start_Before_Or_On_The_End()
    {
        await TimeRange.TryFrom(Nine, FiveThirty, out var range).Should().BeTrue();
        await range.Should().BeEqualTo(TimeRange.From(Nine, FiveThirty));
        await range!.Duration.Should().BeEqualTo(TimeSpan.FromHours(8.5));
        await TimeRange.From(Nine, Nine).Duration.Should().BeEqualTo(TimeSpan.Zero);
    }

    [Test]
    public async Task Rejects_A_Range_That_Runs_Past_Midnight()
    {
        await TimeRange.TryFrom(new(22, 0), new(2, 0), out var range).Should().BeFalse();
        await range.Should().BeNull();
        await ( (Action)( () => TimeRange.From(new(22, 0), new(2, 0)) ) ).Should().Throw<ArgumentException>();
    }

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(TimeRange.From(Nine, FiveThirty), """{"start":"09:00:00","end":"17:30:00"}""");
        await FailsToRead<TimeRange>("""{"start":"17:30:00","end":"09:00:00"}""");
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
    public async Task Accepts_Any_Amount(double amount)
    {
        await Money.TryFrom((decimal)amount, Usd, out var money).Should().BeTrue();
        await money.Should().BeEqualTo(Money.From((decimal)amount, Usd));
        await money!.Amount.Should().BeEqualTo((decimal)amount);
        await money.Currency.Should().BeEqualTo(Usd);
    }

    [Test]
    public async Task Rejects_No_Currency()
    {
        await Money.TryFrom(1m, null, out var money).Should().BeFalse();
        await money.Should().BeNull();
        await ( (Action)( () => Money.From(1m, null!) ) ).Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task Compares_Amount_And_Currency()
    {
        await Money.From(1m, Usd).Should().BeEqualTo(Money.From(1m, CurrencyCode.From("USD")));
        await Money.From(1m, Usd).Should().NotBeEqualTo(Money.From(1m, CurrencyCode.From("EUR")));
        await Money.From(1m, Usd).ToString().Should().BeEqualTo("1 USD");
    }

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Money.From(12.5m, Usd), """{"amount":12.5,"currency":"USD"}""");
        await FailsToRead<Money>("""{"amount":12.5,"currency":"usd"}""");
        await FailsToRead<Money>("""{"amount":12.5}""");
    }
}
