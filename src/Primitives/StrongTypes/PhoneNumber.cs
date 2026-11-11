using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a phone number: an ITU-T E.164 number in its international form, <c>+</c> followed by the country code and the
///     subscriber number, such as <c>+14155552671</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>Digits only after the <c>+</c>: no spaces, hyphens, brackets or extensions.</item>
///         <item>The first digit (the start of the country code) is not 0, and there are 7 to 15 digits (E.164 §6.1 sets the maximum).</item>
///         <item>The country code is not checked against the ITU assignments, nor the number against national numbering plans.</item>
///     </list>
/// </remarks>
public sealed partial class PhoneNumberRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType.Define<string>().MatchesRegex(Pattern());

    [GeneratedRegex(@"^\+[1-9][0-9]{6,14}\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private PhoneNumberRules() { }
}

/// <summary>A phone number in E.164 form, valid under <see cref="PhoneNumberRules" />.</summary>
[StrongType<string, PhoneNumberRules>]
public sealed partial class PhoneNumber;
