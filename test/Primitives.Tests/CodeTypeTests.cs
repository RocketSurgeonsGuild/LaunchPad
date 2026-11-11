using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class CountryCodeTests
{
    [Test]
    [Arguments("US")]
    [Arguments("GB")]
    [Arguments("DE")]
    [Arguments("EH")]
    [Arguments("AQ")]
    [Arguments("SS")]
    [Arguments("ZW")]
    public void Accepts(string value) => Accepts<CountryCode, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("us")]
    [Arguments("Us")]
    [Arguments("USA")]
    [Arguments("U")]
    [Arguments("XK")]
    [Arguments("ZZ")]
    [Arguments("UK")]
    [Arguments("EU")]
    [Arguments("AN")]
    [Arguments("CS")]
    [Arguments("YU")]
    [Arguments("12")]
    public void Rejects(string value) => Rejects<CountryCode, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(CountryCode.From("US"), "\"US\"");
        FailsToRead<CountryCode>("\"us\"");
    }

    [Test]
    public void Round_Trips_As_A_Dictionary_Key() => RoundTrips(
        new Dictionary<CountryCode, Money> { [CountryCode.From("US")] = Money.From(1.5m, CurrencyCode.From("USD")) },
        """{"US":{"amount":1.5,"currency":"USD"}}"""
    );
}

public class CurrencyCodeTests
{
    [Test]
    [Arguments("USD")]
    [Arguments("EUR")]
    [Arguments("JPY")]
    [Arguments("CHF")]
    [Arguments("USN")]
    [Arguments("XAU")]
    [Arguments("XCG")]
    [Arguments("ZWG")]
    [Arguments("SLE")]
    [Arguments("XTS")]
    [Arguments("XXX")]
    public void Accepts(string value) => Accepts<CurrencyCode, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("usd")]
    [Arguments("Usd")]
    [Arguments("US")]
    [Arguments("USDD")]
    [Arguments("ABC")]
    [Arguments("HRK")]
    [Arguments("BGN")]
    [Arguments("ANG")]
    [Arguments("ZWL")]
    [Arguments("SLL")]
    [Arguments("DEM")]
    [Arguments("840")]
    public void Rejects(string value) => Rejects<CurrencyCode, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(CurrencyCode.From("EUR"), "\"EUR\"");
        FailsToRead<CurrencyCode>("\"eur\"");
    }
}

public class LanguageCodeTests
{
    [Test]
    [Arguments("en")]
    [Arguments("en-US")]
    [Arguments("de-CH")]
    [Arguments("fr")]
    [Arguments("zh-Hans")]
    [Arguments("zh-Hans-CN")]
    [Arguments("sr-Latn-RS")]
    [Arguments("gsw")]
    public void Accepts(string value) => Accepts<LanguageCode, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("EN")]
    [Arguments("en-us")]
    [Arguments("EN-us")]
    [Arguments("en_US")]
    [Arguments("xx")]
    [Arguments("en-XX")]
    [Arguments("tlh")]
    [Arguments("i-klingon")]
    [Arguments("x-private")]
    [Arguments("english")]
    public void Rejects(string value) => Rejects<LanguageCode, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(LanguageCode.From("en-US"), "\"en-US\"");
        FailsToRead<LanguageCode>("\"en_US\"");
    }
}
