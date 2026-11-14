using System.Globalization;

using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class ExpirationDateTests
{
    [Test]
    [Arguments("2020-01-31")]
    [Arguments("2030-12-31")]
    [Arguments("9999-12-31")]
    [Arguments("0001-01-02")]
    public async Task Accepts(string value) => await Accepts<ExpirationDate, DateOnly>(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    [Test]
    public async Task Rejects_The_Unset_Date() => await Rejects<ExpirationDate, DateOnly>(DateOnly.MinValue);

    [Test]
    public async Task Expires_The_Day_After()
    {
        var expiration = ExpirationDate.From(new(2030, 12, 31));
        await expiration.IsExpiredOn(new(2030, 12, 30)).Should().BeFalse();
        await expiration.IsExpiredOn(new(2030, 12, 31)).Should().BeFalse();
        await expiration.IsExpiredOn(new(2031, 1, 1)).Should().BeTrue();
    }

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(ExpirationDate.From(new(2030, 12, 31)), "\"2030-12-31\"");
        await FailsToRead<ExpirationDate>("\"0001-01-01\"");
    }
}

public class BusinessDateTests
{
    [Test]
    [Arguments("2024-01-15")] // Monday
    [Arguments("2024-01-17")] // Wednesday
    [Arguments("2024-01-19")] // Friday
    [Arguments("2024-12-25")] // a holiday on a Wednesday: holidays are not checked
    public async Task Accepts(string value) => await Accepts<BusinessDate, DateOnly>(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    [Test]
    [Arguments("2024-01-13")] // Saturday
    [Arguments("2024-01-14")] // Sunday
    public async Task Rejects(string value) => await Rejects<BusinessDate, DateOnly>(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(BusinessDate.From(new(2024, 1, 15)), "\"2024-01-15\"");
        await FailsToRead<BusinessDate>("\"2024-01-13\"");
    }
}
