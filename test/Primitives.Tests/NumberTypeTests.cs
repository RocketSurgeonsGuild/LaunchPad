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
    public void Accepts(double value) => Accepts<Percentage, decimal>((decimal)value);

    [Test]
    [Arguments(-0.01d)]
    [Arguments(100.01d)]
    public void Rejects(double value) => Rejects<Percentage, decimal>((decimal)value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Percentage.From(12.5m), "12.5");
        FailsToRead<Percentage>("101");
    }
}

public class PriceTests
{
    [Test]
    [Arguments(0d)]
    [Arguments(0.0001d)]
    [Arguments(19.99d)]
    [Arguments(1000000d)]
    public void Accepts(double value) => Accepts<Price, decimal>((decimal)value);

    [Test]
    [Arguments(-0.01d)]
    [Arguments(-100d)]
    public void Rejects(double value) => Rejects<Price, decimal>((decimal)value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Price.From(19.99m), "19.99");
        FailsToRead<Price>("-1");
    }
}

public class TaxRateTests
{
    [Test]
    [Arguments(0d)]
    [Arguments(7.25d)]
    [Arguments(100d)]
    public void Accepts(double value) => Accepts<TaxRate, decimal>((decimal)value);

    [Test]
    [Arguments(-0.5d)]
    [Arguments(100.0001d)]
    public void Rejects(double value) => Rejects<TaxRate, decimal>((decimal)value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(TaxRate.From(7.25m), "7.25");
        FailsToRead<TaxRate>("-7");
    }
}

public class QuantityTests
{
    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(int.MaxValue)]
    public void Accepts(int value) => Accepts<Quantity, int>(value);

    [Test]
    [Arguments(-1)]
    [Arguments(int.MinValue)]
    public void Rejects(int value) => Rejects<Quantity, int>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Quantity.From(3), "3");
        FailsToRead<Quantity>("-3");
    }
}

public class RatingTests
{
    [Test]
    [Arguments(1)]
    [Arguments(3)]
    [Arguments(5)]
    public void Accepts(int value) => Accepts<Rating, int>(value);

    [Test]
    [Arguments(0)]
    [Arguments(6)]
    [Arguments(-1)]
    public void Rejects(int value) => Rejects<Rating, int>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Rating.From(4), "4");
        FailsToRead<Rating>("0");
    }
}

public class LatitudeTests
{
    [Test]
    [Arguments(-90d)]
    [Arguments(0d)]
    [Arguments(51.5074d)]
    [Arguments(90d)]
    public void Accepts(double value) => Accepts<Latitude, double>(value);

    [Test]
    [Arguments(-90.0001d)]
    [Arguments(90.0001d)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    [Arguments(double.NegativeInfinity)]
    public void Rejects(double value) => Rejects<Latitude, double>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Latitude.From(51.5074), "51.5074");
        FailsToRead<Latitude>("91");
    }
}

public class LongitudeTests
{
    [Test]
    [Arguments(-180d)]
    [Arguments(-0.1278d)]
    [Arguments(180d)]
    public void Accepts(double value) => Accepts<Longitude, double>(value);

    [Test]
    [Arguments(-180.0001d)]
    [Arguments(180.0001d)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    public void Rejects(double value) => Rejects<Longitude, double>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Longitude.From(-0.1278), "-0.1278");
        FailsToRead<Longitude>("181");
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
    public void Accepts(double value) => Accepts<Temperature, double>(value);

    [Test]
    [Arguments(-273.16d)]
    [Arguments(-300d)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    public void Rejects(double value) => Rejects<Temperature, double>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Temperature.From(-40), "-40");
        FailsToRead<Temperature>("-274");
    }
}

public class MonthTests
{
    [Test]
    [Arguments(1)]
    [Arguments(6)]
    [Arguments(12)]
    public void Accepts(int value) => Accepts<Month, int>(value);

    [Test]
    [Arguments(0)]
    [Arguments(13)]
    public void Rejects(int value) => Rejects<Month, int>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Month.From(12), "12");
        FailsToRead<Month>("13");
    }
}

public class QuarterTests
{
    [Test]
    [Arguments(1)]
    [Arguments(4)]
    public void Accepts(int value) => Accepts<Quarter, int>(value);

    [Test]
    [Arguments(0)]
    [Arguments(5)]
    public void Rejects(int value) => Rejects<Quarter, int>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Quarter.From(2), "2");
        FailsToRead<Quarter>("5");
    }
}
