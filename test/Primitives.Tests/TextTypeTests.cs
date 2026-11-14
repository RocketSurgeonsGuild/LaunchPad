using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class EmailAddressTests
{
    [Test]
    [Arguments("user@example.com")]
    [Arguments("first.last@example.co.uk")]
    [Arguments("user+tag@sub.example.org")]
    [Arguments("o'brien@example.ie")]
    [Arguments("!#$%&'*+/=?^_`{|}~-@example.com")]
    [Arguments("User@Example.COM")]
    [Arguments("user@xn--bcher-kva.example")]
    [Arguments("a@b-c.io")]
    public async Task Accepts(string value) => await Accepts<EmailAddress, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("user")]
    [Arguments("user@")]
    [Arguments("@example.com")]
    [Arguments("user@localhost")]
    [Arguments("user@[192.0.2.1]")]
    [Arguments("\"quoted\"@example.com")]
    [Arguments(".user@example.com")]
    [Arguments("user.@example.com")]
    [Arguments("us..er@example.com")]
    [Arguments("user@-example.com")]
    [Arguments("user@example-.com")]
    [Arguments("user@example.123")]
    [Arguments("user@exa mple.com")]
    [Arguments("user@example.com\n")]
    [Arguments("üser@example.com")]
    [Arguments("user@bücher.example")]
    [Arguments("a@b@example.com")]
    public async Task Rejects(string value) => await Rejects<EmailAddress, string>(value);

    [Test]
    public async Task Accepts_A_Local_Part_Of_64_And_Rejects_65()
    {
        await Accepts<EmailAddress, string>(new string('a', 64) + "@example.com");
        await Rejects<EmailAddress, string>(new string('a', 65) + "@example.com");
    }

    [Test]
    public async Task Accepts_254_Characters_And_Rejects_255()
    {
        // 64 + 1 + 189 = 254: the domain is three labels of 61 and a top-level label of 3.
        var domain = $"{new string('b', 61)}.{new string('c', 61)}.{new string('d', 61)}.abc";
        await Accepts<EmailAddress, string>($"{new string('a', 64)}@{domain}");
        await Rejects<EmailAddress, string>($"{new string('a', 64)}@{domain}e");
    }

    [Test]
    public async Task Keeps_Case() => await EmailAddress.From("User@example.com").Should().NotBeEqualTo(EmailAddress.From("user@example.com"));

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(EmailAddress.From("user@example.com"), "\"user@example.com\"");
        await FailsToRead<EmailAddress>("\"not an email\"");
    }
}

public class PhoneNumberTests
{
    [Test]
    [Arguments("+14155552671")]
    [Arguments("+442071838750")]
    [Arguments("+6834001")]
    [Arguments("+123456789012345")]
    public async Task Accepts(string value) => await Accepts<PhoneNumber, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("14155552671")]
    [Arguments("+04155552671")]
    [Arguments("+1 415 555 2671")]
    [Arguments("+1-415-555-2671")]
    [Arguments("+1(415)5552671")]
    [Arguments("+123456")]
    [Arguments("+1234567890123456")]
    [Arguments("+1415555267x")]
    public async Task Rejects(string value) => await Rejects<PhoneNumber, string>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        // The default encoder escapes the plus sign.
        await RoundTrips(PhoneNumber.From("+14155552671"), "\"\\u002B14155552671\"");
        await FailsToRead<PhoneNumber>("\"4155552671\"");
    }
}

public class UrlTests
{
    [Test]
    [Arguments("https://example.com")]
    [Arguments("http://example.com/")]
    [Arguments("https://example.com:8443/path/to?query=1&b=two#fragment")]
    [Arguments("https://user@example.com/")]
    [Arguments("https://192.0.2.1/")]
    [Arguments("https://[2001:db8::1]/")]
    [Arguments("https://example.com/a%20b")]
    [Arguments("https://Example.COM/Path")]
    [Arguments("https://xn--bcher-kva.example/")]
    public async Task Accepts(string value) => await Accepts<Url, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("example.com")]
    [Arguments("/relative/path")]
    [Arguments("ftp://example.com/")]
    [Arguments("mailto:user@example.com")]
    [Arguments("HTTPS://example.com/")]
    [Arguments("https://")]
    [Arguments("https://example.com/a b")]
    [Arguments("https://example.com/%zz")]
    [Arguments("https://bücher.example/")]
    [Arguments("https://example.com/\n")]
    public async Task Rejects(string value) => await Rejects<Url, string>(value);

    [Test]
    public async Task Accepts_8000_Characters_And_Rejects_8001()
    {
        const string prefix = "https://example.com/";
        await Accepts<Url, string>(prefix + new string('a', UrlRules.MaxLength - prefix.Length));
        await Rejects<Url, string>(prefix + new string('a', UrlRules.MaxLength - prefix.Length + 1));
    }

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Url.From("https://example.com/a?b=c"), "\"https://example.com/a?b=c\"");
        await FailsToRead<Url>("\"ftp://example.com/\"");
    }
}

public class SlugTests
{
    [Test]
    [Arguments("hello")]
    [Arguments("hello-world")]
    [Arguments("hello-world-2")]
    [Arguments("2024")]
    [Arguments("a")]
    public async Task Accepts(string value) => await Accepts<Slug, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("Hello-World")]
    [Arguments("hello_world")]
    [Arguments("hello world")]
    [Arguments("-hello")]
    [Arguments("hello-")]
    [Arguments("hello--world")]
    [Arguments("héllo")]
    public async Task Rejects(string value) => await Rejects<Slug, string>(value);

    [Test]
    public async Task Accepts_100_Characters_And_Rejects_101()
    {
        await Accepts<Slug, string>(new string('a', 100));
        await Rejects<Slug, string>(new string('a', 101));
    }

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(Slug.From("hello-world"), "\"hello-world\"");
        await FailsToRead<Slug>("\"Hello World\"");
    }
}

public class HexColorTests
{
    [Test]
    [Arguments("#fff")]
    [Arguments("#000000")]
    [Arguments("#1a2b3c")]
    public async Task Accepts(string value) => await Accepts<HexColor, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("#FFF")]
    [Arguments("#1A2B3C")]
    [Arguments("fff")]
    [Arguments("#ffff")]
    [Arguments("#ffffff00")]
    [Arguments("#ff")]
    [Arguments("#gggggg")]
    [Arguments("#fffffff")]
    public async Task Rejects(string value) => await Rejects<HexColor, string>(value);

    [Test]
    public async Task Round_Trips_Through_Json()
    {
        await RoundTrips(HexColor.From("#1a2b3c"), "\"#1a2b3c\"");
        await FailsToRead<HexColor>("\"#1A2B3C\"");
    }
}
