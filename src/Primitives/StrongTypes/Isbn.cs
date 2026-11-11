using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of an ISBN, an international standard book number (ISO 2108): an ISBN-13 such as <c>9780306406157</c> or an ISBN-10 such
///     as <c>0306406152</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             The compact form: digits only, without the hyphens or spaces of the printed form, and no <c>ISBN</c> prefix. An ISBN-10 may end
///             in an upper-case <c>X</c> (check value 10); lower case <c>x</c> is rejected.
///         </item>
///         <item>An ISBN-13 starts with the 978 or 979 prefix of the book industry.</item>
///         <item>
///             The check digit must be right: weights 10 to 1 modulo 11 for an ISBN-10, weights 1 and 3 modulo 10 for an ISBN-13. The
///             registration group and registrant are not checked against the ISBN ranges.
///         </item>
///         <item>ISBN-10 and ISBN-13 values are kept as given; the same book in both forms is two different values.</item>
///     </list>
/// </remarks>
public sealed partial class IsbnRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .MatchesRegex(Pattern())
                                                         .Satisfies(HasValidCheckDigit, "Value must have a valid ISBN check digit.");

    // Reported by the pattern when the value is not well-formed.
    private static bool HasValidCheckDigit(string value)
    {
        if (!Pattern().IsMatch(value)) return true;

        var sum = 0;
        if (value.Length == 10)
        {
            for (var i = 0; i < 10; i++)
            {
                sum += ( 10 - i ) * ( value[i] == 'X' ? 10 : value[i] - '0' );
            }

            return sum % 11 == 0;
        }

        for (var i = 0; i < 13; i++)
        {
            sum += ( i % 2 == 0 ? 1 : 3 ) * ( value[i] - '0' );
        }

        return sum % 10 == 0;
    }

    [GeneratedRegex(@"^(?:[0-9]{9}[0-9X]|97[89][0-9]{10})\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private IsbnRules() { }
}

/// <summary>An ISBN-10 or ISBN-13, valid under <see cref="IsbnRules" />.</summary>
[StrongType<string, IsbnRules>]
public sealed partial class Isbn;
