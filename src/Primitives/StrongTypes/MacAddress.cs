using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a MAC address, an IEEE EUI-48, in the IEEE 802 hexadecimal representation: six groups of two upper-case hexadecimal
///     digits separated by hyphens, such as <c>00-1A-2B-3C-4D-5E</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             IEEE 802 §8.1 writes each octet as two upper-case hexadecimal digits separated by hyphens. Colons (<c>00:1a:2b:3c:4d:5e</c>),
///             dots, lower case and the bare 12 digits are rejected rather than converted, since SuperStrong validates without changing the
///             value.
///         </item>
///         <item>EUI-64 addresses are rejected. Any EUI-48 is accepted, including broadcast and multicast addresses.</item>
///     </list>
/// </remarks>
public sealed partial class MacAddressRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType.Define<string>().MatchesRegex(Pattern());

    [GeneratedRegex(@"^[0-9A-F]{2}(?:-[0-9A-F]{2}){5}\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private MacAddressRules() { }
}

/// <summary>An EUI-48 MAC address, valid under <see cref="MacAddressRules" />.</summary>
[StrongType<string, MacAddressRules>]
public sealed partial class MacAddress;
