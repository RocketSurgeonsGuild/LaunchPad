using System.Net;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of an IP address in its canonical text form: IPv4 dotted decimal (<c>192.0.2.1</c>) or IPv6 as RFC 5952 recommends
///     (<c>2001:db8::1</c>).
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             The value must be exactly what <see cref="IPAddress.ToString" /> writes for it, so each address has one value: IPv4 without
///             leading zeros or shortened forms (<c>127.1</c>), IPv6 in lower case with the longest run of zero groups compressed
///             (RFC 5952 §4). <c>2001:DB8::1</c> and <c>2001:0db8:0:0:0:0:0:1</c> are rejected.
///         </item>
///         <item>IPv6 zone identifiers (<c>fe80::1%eth0</c>) are rejected; they only mean something on the host that wrote them.</item>
///         <item>A prefix length (CIDR notation) is not an address and is rejected.</item>
///     </list>
/// </remarks>
public sealed class IpAddressRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .Satisfies(IsCanonical, "Value must be an IPv4 or IPv6 address in canonical form.");

    private static bool IsCanonical(string value) =>
        !value.Contains('%', StringComparison.Ordinal)
     && IPAddress.TryParse(value, out var address)
     && string.Equals(address.ToString(), value, StringComparison.Ordinal);

    private IpAddressRules() { }
}

/// <summary>An IPv4 or IPv6 address, valid under <see cref="IpAddressRules" />.</summary>
[StrongType<string, IpAddressRules>]
public sealed partial class IpAddress
{
    /// <summary>Returns the address as an <see cref="IPAddress" />.</summary>
    public IPAddress ToIPAddress() => IPAddress.Parse(AsPrimitive());
}
