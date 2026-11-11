using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a VIN, a vehicle identification number (ISO 3779), such as <c>1M8GDM9AXKP042788</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             17 characters: digits and upper-case letters except <c>I</c>, <c>O</c> and <c>Q</c> (ISO 3779 §5.4). Lower case is rejected.
///         </item>
///         <item>
///             The ninth character is the check digit (<c>0</c>-<c>9</c> or <c>X</c>) computed as North America requires (49 CFR 565.15).
///             ISO 3779 does not require a check digit, so VINs of vehicles made for markets that do not use one can be rejected; an
///             application that has them declares its own type with rules that leave the check out.
///         </item>
///         <item>The manufacturer identifier is not checked against the SAE registry, nor the model year character.</item>
///     </list>
/// </remarks>
public sealed partial class VinRules : IStrongTypeTemplate<string>
{
    private static readonly int[] Weights = [8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2];

    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .MatchesRegex(Pattern())
                                                         .Satisfies(HasValidCheckDigit, "Value must have a valid VIN check digit.");

    // Reported by the pattern when the value is not well-formed.
    private static bool HasValidCheckDigit(string value)
    {
        if (!Pattern().IsMatch(value)) return true;

        var sum = 0;
        for (var i = 0; i < value.Length; i++)
        {
            sum += Weights[i] * Transliterate(value[i]);
        }

        var check = sum % 11;
        return value[8] == ( check == 10 ? 'X' : (char)( '0' + check ) );
    }

    private static int Transliterate(char character) => character switch
    {
        >= '0' and <= '9' => character - '0',
        >= 'A' and <= 'H' => character - 'A' + 1,
        >= 'J' and <= 'N' => character - 'J' + 1,
        'P' => 7,
        'R' => 9,
        >= 'S' and <= 'Z' => character - 'S' + 2,
        _ => 0,
    };

    [GeneratedRegex(@"^[A-HJ-NPR-Z0-9]{17}\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private VinRules() { }
}

/// <summary>A vehicle identification number, valid under <see cref="VinRules" />.</summary>
[StrongType<string, VinRules>]
public sealed partial class Vin;
