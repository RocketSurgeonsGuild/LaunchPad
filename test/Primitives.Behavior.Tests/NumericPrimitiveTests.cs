using static Primitives.Behavior.Tests.PrimitiveAssert;

namespace Primitives.Behavior.Tests;

public class NumericPrimitiveTests
{
    [Test]
    public async Task Should_Accept_A_Value_In_Its_Range()
    {
        await Accepts<Score, int>(0);
        await Accepts<Score, int>(100);
        await Accepts<Points, int>(50);
        await Accepts<Distance, double>(999.5);
        await Accepts<Measurement, double>(-1e300);
    }

    [Test]
    public async Task Should_Reject_A_Value_Outside_Its_Range()
    {
        var below = await Rejects<Score, int>(-1, "RANGE");
        await below.Message.Should().BeEqualTo("Score must be at least 0. Got -1.");
        var above = await Rejects<Score, int>(101, "RANGE");
        await above.Message.Should().BeEqualTo("Score must be at most 100. Got 101.");
        await Rejects<Points, int>(101, "RANGE");
        await Rejects<Ratio, float>(1.5f, "RANGE");
    }

    [Test]
    public async Task Should_Exclude_The_Bounds_When_Exclusive()
    {
        await Rejects<Weight, decimal>(0m, "OUT_OF_BOUNDS");
        await Accepts<Weight, decimal>(0.001m);
        await Accepts<Weight, decimal>(9.999m);
        var error = await Rejects<Weight, decimal>(10m, "OUT_OF_BOUNDS");
        await error.Message.Should().BeEqualTo("Between 0 and 10, exclusive.");
    }

    [Test]
    public async Task Should_Ignore_A_Bound_Past_The_Limit_Of_Its_Type()
    {
        // PrimitiveRange(1, 300) on a byte: 300 can't be reached, so only the minimum checks anything, and only it is described
        await Accepts<Level, byte>(byte.MaxValue);
        await Rejects<Level, byte>(0, "RANGE");
        await Level.Description.Minimum.Should().BeEqualTo("1");
        await Level.Description.Maximum.Should().BeNull();
    }

    [Test]
    public async Task Should_Reject_A_Number_That_Is_Not_Finite()
    {
        await Rejects<Distance, double>(double.NaN, "RANGE");
        await Rejects<Distance, double>(double.PositiveInfinity, "RANGE");
        await Rejects<Measurement, double>(double.NaN, "RANGE");
        await Rejects<Measurement, double>(double.NegativeInfinity, "RANGE");
        await Rejects<Ratio, float>(float.NaN, "RANGE");
    }

    [Test]
    public async Task Should_Check_The_Result_Of_Every_Operator_Again()
    {
        var points = Points.Create(40) + Points.Create(50);
        await points.Value.Should().BeEqualTo(90);
        await ( Points.Create(50) - Points.Create(20) ).Value.Should().BeEqualTo(30);
        await ( Points.Create(20) * 3 ).Value.Should().BeEqualTo(60);
        await ( Points.Create(60) / 4 ).Value.Should().BeEqualTo(15);

        await Throws<Rocket.Surgery.LaunchPad.Primitives.DomainPrimitiveValidationException>(() => _ = Points.Create(60) + Points.Create(50));
        await Throws<Rocket.Surgery.LaunchPad.Primitives.DomainPrimitiveValidationException>(() => _ = Points.Create(10) - Points.Create(20));
        await Throws<Rocket.Surgery.LaunchPad.Primitives.DomainPrimitiveValidationException>(() => _ = -Points.Create(10));
        await ( -Points.Create(0) ).Value.Should().BeEqualTo(0);

        var distance = Distance.Create(400) + Distance.Create(500);
        await distance.Value.Should().BeEqualTo(900d);
        await ( Distance.Create(100) * 2.5 ).Value.Should().BeEqualTo(250d);
        await ( Distance.Create(100) / 4 ).Value.Should().BeEqualTo(25d);
        await Throws<Rocket.Surgery.LaunchPad.Primitives.DomainPrimitiveValidationException>(() => _ = Distance.Create(600) + Distance.Create(600));
        // dividing by zero is an infinity, which is not finite
        await Throws<Rocket.Surgery.LaunchPad.Primitives.DomainPrimitiveValidationException>(() => _ = Distance.Create(1) / 0);

        await ( Ticks.Create(10) - Ticks.Create(30) ).Value.Should().BeEqualTo(-20L);
        await ( Weight.Create(2m) * 2m ).Value.Should().BeEqualTo(4m);
        await ( -Counter.Create(5) ).Value.Should().BeEqualTo(-5);
    }

    [Test]
    public async Task Should_Throw_On_Overflow()
    {
        await Throws<OverflowException>(() => _ = Counter.Create(int.MaxValue) + Counter.Create(1));
        await Throws<OverflowException>(() => _ = Counter.Create(int.MaxValue) * 2);
        await Throws<OverflowException>(() => _ = -Counter.Create(int.MinValue));
    }

    [Test]
    public async Task Should_Throw_When_An_Operand_Is_Default()
    {
        await Throws<InvalidOperationException>(() => _ = default(Counter) + Counter.Create(1));
        await Throws<InvalidOperationException>(() => _ = (int)default(Score));
    }

    [Test]
    public async Task Should_Describe_Its_Range()
    {
        await Score.Description.Minimum.Should().BeEqualTo("0");
        await Score.Description.Maximum.Should().BeEqualTo("100");
        await Weight.Description.MinimumIsExclusive.Should().BeTrue();
        await Weight.Description.MaximumIsExclusive.Should().BeTrue();
        await MovieRating.Description.Scale.Should().BeEqualTo(2);
        await Measurement.Description.Minimum.Should().BeNull();
    }
}

public class NumericKindTests
{
    [Test]
    public async Task Should_Validate_A_Percentage()
    {
        await Accepts<CompletionRate, decimal>(0m);
        await Accepts<CompletionRate, decimal>(100m);
        await Rejects<CompletionRate, decimal>(100.01m, "RANGE");
        await Rejects<CompletionRate, decimal>(-0.01m, "RANGE");
        await Accepts<Commission, decimal>(15m);
        await Rejects<Commission, decimal>(9m, "RANGE");
    }

    [Test]
    public async Task Should_Validate_Coordinates()
    {
        await Accepts<Latitude, double>(-90);
        await Accepts<Latitude, double>(90);
        await Rejects<Latitude, double>(90.0001, "RANGE");
        await Accepts<Longitude, double>(180);
        await Rejects<Longitude, double>(-180.5, "RANGE");
        await Rejects<Longitude, double>(double.NaN, "RANGE");
    }

    [Test]
    public async Task Should_Validate_A_Rating_And_Its_Scale()
    {
        await Accepts<MovieRating, decimal>(4.25m);
        var scale = await Rejects<MovieRating, decimal>(4.255m, "SCALE");
        await scale.Message.Should().BeEqualTo("MovieRating must have at most 2 decimal place(s).");
        await Rejects<MovieRating, decimal>(5.01m, "RANGE");
    }

    [Test]
    public async Task Should_Validate_Calendar_Numbers()
    {
        await Accepts<MonthOfYear, int>(1);
        await Accepts<MonthOfYear, int>(12);
        await Rejects<MonthOfYear, int>(13, "RANGE");
        await Rejects<MonthOfYear, int>(0, "RANGE");
        await Accepts<QuarterOfYear, int>(4);
        await Rejects<QuarterOfYear, int>(5, "RANGE");
        await Accepts<IsoWeek, int>(53);
        await Rejects<IsoWeek, int>(54, "RANGE");
        // without ISO numbering a year can have a 54th week
        await Accepts<CalendarWeek, int>(54);
        await Rejects<CalendarWeek, int>(55, "RANGE");
        await Accepts<CompanyFiscalYear, int>(2000);
        await Rejects<CompanyFiscalYear, int>(1999, "RANGE");
        await Rejects<CompanyFiscalYear, int>(2101, "RANGE");
    }

    [Test]
    public async Task Should_Validate_Measurements()
    {
        await Accepts<PersonAge, int>(150);
        await Rejects<PersonAge, int>(151, "RANGE");
        await Rejects<PersonAge, int>(-1, "RANGE");
        await Accepts<ItemCount, int>(int.MaxValue);
        await Rejects<ItemCount, int>(-1, "RANGE");
        await Accepts<UnitPrice, decimal>(0m);
        await Accepts<UnitPrice, decimal>(decimal.MaxValue);
        await Rejects<UnitPrice, decimal>(-0.01m, "RANGE");
        await Accepts<Celsius, double>(-273.15);
        await Rejects<Celsius, double>(-273.16, "RANGE");
    }
}
