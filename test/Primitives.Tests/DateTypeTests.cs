using System.Globalization;

using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class BirthDateTests
{
    private static DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);

    [Test]
    public void Accepts_Today_And_Past_Dates()
    {
        Accepts<BirthDate, DateOnly>(UtcToday);
        Accepts<BirthDate, DateOnly>(new(1990, 5, 17));
        Accepts<BirthDate, DateOnly>(UtcToday.AddYears(-149));
    }

    [Test]
    public void Accepts_Today_Anywhere_On_Earth() => Accepts<BirthDate, DateOnly>(UtcToday.AddDays(1).AddDays(DateTime.UtcNow.Hour >= 10 ? 0 : -1));

    [Test]
    public void Rejects_The_Future()
    {
        Rejects<BirthDate, DateOnly>(UtcToday.AddDays(2));
        Rejects<BirthDate, DateOnly>(DateOnly.MaxValue);
    }

    [Test]
    public void Rejects_More_Than_150_Years_Ago()
    {
        Rejects<BirthDate, DateOnly>(UtcToday.AddYears(-150).AddDays(-2));
        Rejects<BirthDate, DateOnly>(DateOnly.MinValue);
    }

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(BirthDate.From(new(1990, 5, 17)), "\"1990-05-17\"");
        FailsToRead<BirthDate>("\"9999-12-31\"");
    }
}

public class ExpirationDateTests
{
    [Test]
    [Arguments("2020-01-31")]
    [Arguments("2030-12-31")]
    [Arguments("9999-12-31")]
    [Arguments("0001-01-02")]
    public void Accepts(string value) => Accepts<ExpirationDate, DateOnly>(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    [Test]
    public void Rejects_The_Unset_Date() => Rejects<ExpirationDate, DateOnly>(DateOnly.MinValue);

    [Test]
    public void Expires_The_Day_After()
    {
        var expiration = ExpirationDate.From(new(2030, 12, 31));
        expiration.IsExpiredOn(new(2030, 12, 30)).ShouldBeFalse();
        expiration.IsExpiredOn(new(2030, 12, 31)).ShouldBeFalse();
        expiration.IsExpiredOn(new(2031, 1, 1)).ShouldBeTrue();
    }

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(ExpirationDate.From(new(2030, 12, 31)), "\"2030-12-31\"");
        FailsToRead<ExpirationDate>("\"0001-01-01\"");
    }
}

public class BusinessDateTests
{
    [Test]
    [Arguments("2024-01-15")] // Monday
    [Arguments("2024-01-17")] // Wednesday
    [Arguments("2024-01-19")] // Friday
    [Arguments("2024-12-25")] // a holiday on a Wednesday: holidays are not checked
    public void Accepts(string value) => Accepts<BusinessDate, DateOnly>(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    [Test]
    [Arguments("2024-01-13")] // Saturday
    [Arguments("2024-01-14")] // Sunday
    public void Rejects(string value) => Rejects<BusinessDate, DateOnly>(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(BusinessDate.From(new(2024, 1, 15)), "\"2024-01-15\"");
        FailsToRead<BusinessDate>("\"2024-01-13\"");
    }
}
