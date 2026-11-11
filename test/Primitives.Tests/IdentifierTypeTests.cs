using System.Net;

using Rocket.Surgery.LaunchPad.StrongTypes;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

public class IbanTests
{
    [Test]
    [Arguments("GB82WEST12345698765432")]
    [Arguments("DE89370400440532013000")]
    [Arguments("NO9386011117947")]
    [Arguments("FR1420041010050500013M02606")]
    [Arguments("LC55HEMM000100010012001200023015")]
    [Arguments("GB02WEST12345698765417")]
    public void Accepts(string value) => Accepts<Iban, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("GB82WEST12345698765433")]
    [Arguments("GB28WEST12345698765432")]
    [Arguments("gb82west12345698765432")]
    [Arguments("GB82 WEST 1234 5698 7654 32")]
    [Arguments("GB82-WEST-1234-5698-7654-32")]
    [Arguments("GBXXWEST12345698765432")]
    [Arguments("1282WEST12345698765432")]
    [Arguments("NO938601111794")]
    [Arguments("GB82WEST1234569876543212345678901")]
    [Arguments("GB99WEST12345698765417")]
    [Arguments("GB00WEST12345698765432")]
    public void Rejects(string value) => Rejects<Iban, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Iban.From("DE89370400440532013000"), "\"DE89370400440532013000\"");
        FailsToRead<Iban>("\"DE89370400440532013001\"");
    }
}

public class IsbnTests
{
    [Test]
    [Arguments("9780306406157")]
    [Arguments("9790000000001")]
    [Arguments("0306406152")]
    [Arguments("080442957X")]
    public void Accepts(string value) => Accepts<Isbn, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("9780306406158")]
    [Arguments("0306406153")]
    [Arguments("080442957x")]
    [Arguments("978-0-306-40615-7")]
    [Arguments("0-306-40615-2")]
    [Arguments("ISBN9780306406157")]
    [Arguments("9770306406158")]
    [Arguments("X306406152")]
    [Arguments("97803064061")]
    public void Rejects(string value) => Rejects<Isbn, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Isbn.From("9780306406157"), "\"9780306406157\"");
        FailsToRead<Isbn>("\"9780306406158\"");
    }
}

public class VinTests
{
    [Test]
    [Arguments("1M8GDM9AXKP042788")]
    [Arguments("11111111111111111")]
    [Arguments("5GZCZ43D13S812715")]
    [Arguments("JHMCM56557C404453")]
    public void Accepts(string value) => Accepts<Vin, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("1M8GDM9A1KP042788")]
    [Arguments("1m8gdm9axkp042788")]
    [Arguments("1M8GDM9AXKP04278")]
    [Arguments("1M8GDM9AXKP0427888")]
    [Arguments("1M8GDM9AXKP0427I8")]
    [Arguments("1M8GDM9AXKP0427O8")]
    [Arguments("1M8GDM9AXKP0427Q8")]
    [Arguments("1M8GDM9AX-P042788")]
    public void Rejects(string value) => Rejects<Vin, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(Vin.From("1M8GDM9AXKP042788"), "\"1M8GDM9AXKP042788\"");
        FailsToRead<Vin>("\"1M8GDM9A1KP042788\"");
    }
}

public class IpAddressTests
{
    [Test]
    [Arguments("192.0.2.1")]
    [Arguments("0.0.0.0")]
    [Arguments("255.255.255.255")]
    [Arguments("2001:db8::1")]
    [Arguments("::1")]
    [Arguments("::")]
    [Arguments("fe80::1")]
    [Arguments("2001:db8:0:1:1:1:1:1")]
    [Arguments("::ffff:192.0.2.1")]
    public void Accepts(string value) => Accepts<IpAddress, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("256.0.0.1")]
    [Arguments("192.168.1")]
    [Arguments("127.1")]
    [Arguments("192.168.001.1")]
    [Arguments(" 192.0.2.1")]
    [Arguments("192.0.2.1/24")]
    [Arguments("2001:DB8::1")]
    [Arguments("2001:0db8::1")]
    [Arguments("2001:db8:0:0:0:0:0:1")]
    [Arguments("[2001:db8::1]")]
    [Arguments("fe80::1%4")]
    [Arguments("fe80::1%eth0")]
    [Arguments("localhost")]
    public void Rejects(string value) => Rejects<IpAddress, string>(value);

    [Test]
    public void Converts_To_An_IPAddress() => IpAddress.From("2001:db8::1").ToIPAddress().ShouldBe(IPAddress.Parse("2001:db8::1"));

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(IpAddress.From("192.0.2.1"), "\"192.0.2.1\"");
        FailsToRead<IpAddress>("\"2001:DB8::1\"");
    }
}

public class MacAddressTests
{
    [Test]
    [Arguments("00-1A-2B-3C-4D-5E")]
    [Arguments("FF-FF-FF-FF-FF-FF")]
    [Arguments("01-00-5E-00-00-FB")]
    public void Accepts(string value) => Accepts<MacAddress, string>(value);

    [Test]
    [Arguments("")]
    [Arguments("00-1a-2b-3c-4d-5e")]
    [Arguments("00:1A:2B:3C:4D:5E")]
    [Arguments("001A.2B3C.4D5E")]
    [Arguments("001A2B3C4D5E")]
    [Arguments("00-1A-2B-3C-4D")]
    [Arguments("00-1A-2B-3C-4D-5E-6F-70")]
    [Arguments("00-1A-2B-3C-4D-5G")]
    public void Rejects(string value) => Rejects<MacAddress, string>(value);

    [Test]
    public void Round_Trips_Through_Json()
    {
        RoundTrips(MacAddress.From("00-1A-2B-3C-4D-5E"), "\"00-1A-2B-3C-4D-5E\"");
        FailsToRead<MacAddress>("\"00:1a:2b:3c:4d:5e\"");
    }
}
