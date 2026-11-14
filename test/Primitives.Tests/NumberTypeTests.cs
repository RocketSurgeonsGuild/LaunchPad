using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

// Attributes cannot hold decimals, so the decimal cases are written as doubles that convert exactly.
public class PercentageTests
{
    [Test]
    [Arguments(0d)]
    [Arguments(12.5d)]
    [Arguments(100d)]
    public async Task Accepts(double value) => await Accepts<Percentage, decimal>((decimal)value);

    [Test]
    [Arguments(-0.01d)]
    [Arguments(100.01d)]
    public async Task Rejects(double value) => await Rejects<Percentage, decimal>((decimal)value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Percentage.From(12.5m), "12.5");
        await FailsToRead<Percentage>("101");
    }
}

public class PriceTests
{
    [Test]
    [Arguments(0d)]
    [Arguments(0.0001d)]
    [Arguments(19.99d)]
    [Arguments(1000000d)]
    public async Task Accepts(double value) => await Accepts<Price, decimal>((decimal)value);

    [Test]
    [Arguments(-0.01d)]
    [Arguments(-100d)]
    public async Task Rejects(double value) => await Rejects<Price, decimal>((decimal)value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Price.From(19.99m), "19.99");
        await FailsToRead<Price>("-1");
    }
}

public class TaxRateTests
{
    [Test]
    [Arguments(0d)]
    [Arguments(7.25d)]
    [Arguments(100d)]
    public async Task Accepts(double value) => await Accepts<TaxRate, decimal>((decimal)value);

    [Test]
    [Arguments(-0.5d)]
    [Arguments(100.0001d)]
    public async Task Rejects(double value) => await Rejects<TaxRate, decimal>((decimal)value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(TaxRate.From(7.25m), "7.25");
        await FailsToRead<TaxRate>("-7");
    }
}

public class QuantityTests
{
    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(int.MaxValue)]
    public async Task Accepts(int value) => await Accepts<Quantity, int>(value);

    [Test]
    [Arguments(-1)]
    [Arguments(int.MinValue)]
    public async Task Rejects(int value) => await Rejects<Quantity, int>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Quantity.From(3), "3");
        await FailsToRead<Quantity>("-3");
    }
}

public class RatingTests
{
    [Test]
    [Arguments(1)]
    [Arguments(3)]
    [Arguments(5)]
    public async Task Accepts(int value) => await Accepts<Rating, int>(value);

    [Test]
    [Arguments(0)]
    [Arguments(6)]
    [Arguments(-1)]
    public async Task Rejects(int value) => await Rejects<Rating, int>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Rating.From(4), "4");
        await FailsToRead<Rating>("0");
    }
}

public class LatitudeTests
{
    [Test]
    [Arguments(-90d)]
    [Arguments(0d)]
    [Arguments(51.5074d)]
    [Arguments(90d)]
    public async Task Accepts(double value) => await Accepts<Latitude, double>(value);

    [Test]
    [Arguments(-90.0001d)]
    [Arguments(90.0001d)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    [Arguments(double.NegativeInfinity)]
    public async Task Rejects(double value) => await Rejects<Latitude, double>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Latitude.From(51.5074), "51.5074");
        await FailsToRead<Latitude>("91");
    }
}

public class LongitudeTests
{
    [Test]
    [Arguments(-180d)]
    [Arguments(-0.1278d)]
    [Arguments(180d)]
    public async Task Accepts(double value) => await Accepts<Longitude, double>(value);

    [Test]
    [Arguments(-180.0001d)]
    [Arguments(180.0001d)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    public async Task Rejects(double value) => await Rejects<Longitude, double>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Longitude.From(-0.1278), "-0.1278");
        await FailsToRead<Longitude>("181");
    }
}

public class TemperatureTests
{
    [Test]
    [Arguments(-273.15d)]
    [Arguments(-40d)]
    [Arguments(0d)]
    [Arguments(36.6d)]
    [Arguments(5500d)]
    public async Task Accepts(double value) => await Accepts<Temperature, double>(value);

    [Test]
    [Arguments(-273.16d)]
    [Arguments(-300d)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    public async Task Rejects(double value) => await Rejects<Temperature, double>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Temperature.From(-40), "-40");
        await FailsToRead<Temperature>("-274");
    }
}

public class MonthTests
{
    [Test]
    [Arguments(1)]
    [Arguments(6)]
    [Arguments(12)]
    public async Task Accepts(int value) => await Accepts<Month, int>(value);

    [Test]
    [Arguments(0)]
    [Arguments(13)]
    public async Task Rejects(int value) => await Rejects<Month, int>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Month.From(12), "12");
        await FailsToRead<Month>("13");
    }
}

public class QuarterTests
{
    [Test]
    [Arguments(1)]
    [Arguments(4)]
    public async Task Accepts(int value) => await Accepts<Quarter, int>(value);

    [Test]
    [Arguments(0)]
    [Arguments(5)]
    public async Task Rejects(int value) => await Rejects<Quarter, int>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Quarter.From(2), "2");
        await FailsToRead<Quarter>("5");
    }
}
