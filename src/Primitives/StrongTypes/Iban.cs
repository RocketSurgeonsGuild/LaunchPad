using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of an IBAN, an international bank account number (ISO 13616-1), such as <c>GB82WEST12345698765432</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             The electronic format: no spaces, two upper-case letters for the country, two check digits, then 11 to 30 upper-case letters
///             and digits, 15 to 34 characters in all. The printed format with spaces, and lower case, are rejected.
///         </item>
///         <item>The check digits are 02 to 98, and the number passes the ISO 7064 MOD 97-10 check.</item>
///         <item>
///             The country's IBAN length and BBAN format (the SWIFT IBAN registry) are not checked, and the country letters are not checked
///             against ISO 3166-1 (the registry includes codes such as <c>XK</c>).
///         </item>
///     </list>
/// </remarks>
public sealed partial class IbanRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .MatchesRegex(Pattern())
                                                         .Satisfies(HasValidCheckDigits, "Value must pass the ISO 7064 MOD 97-10 check.");

    // Reported by the pattern when the value is not well-formed.
    private static bool HasValidCheckDigits(string value)
    {
        if (!Pattern().IsMatch(value)) return true;
        if (value[2..4] is "00" or "01" or "99") return false;

        var remainder = 0;
        for (var i = 0; i < value.Length; i++)
        {
            // The check moves the country code and check digits to the end and turns each letter into two digits (A = 10, ..., Z = 35).
            var character = value[( i + 4 ) % value.Length];
            var digits = character is >= '0' and <= '9' ? character - '0' : character - 'A' + 10;
            var shift = digits < 10 ? 10 : 100;
            remainder = ( remainder * shift + digits ) % 97;
        }

        return remainder == 1;
    }

    [GeneratedRegex(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private IbanRules() { }
}

/// <summary>An IBAN in electronic format, valid under <see cref="IbanRules" />.</summary>
[StrongType<string, IbanRules>]
public sealed partial class Iban;
