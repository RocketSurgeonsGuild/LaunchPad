using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a hexadecimal color: the CSS Color 4 §5.2 hex notation <c>#RGB</c> or <c>#RRGGBB</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>The forms with an alpha channel (<c>#RGBA</c>, <c>#RRGGBBAA</c>) are rejected.</item>
///         <item>
///             CSS reads hex digits in either case; these rules take the lower case HTML uses for a color (the "valid lowercase simple color"
///             of the HTML standard) and reject upper case, so equal colors are equal values. <c>#fff</c> and <c>#ffffff</c> are still
///             different values.
///         </item>
///     </list>
/// </remarks>
public sealed partial class HexColorRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType.Define<string>().MatchesRegex(Pattern());

    [GeneratedRegex(@"^#(?:[0-9a-f]{3}|[0-9a-f]{6})\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private HexColorRules() { }
}

/// <summary>A hexadecimal color, valid under <see cref="HexColorRules" />.</summary>
[StrongType<string, HexColorRules>]
public sealed partial class HexColor;
