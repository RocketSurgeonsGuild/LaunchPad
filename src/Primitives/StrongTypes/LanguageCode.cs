using System.Globalization;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a language code: a BCP 47 language tag that .NET knows as a culture, such as <c>en</c>, <c>en-US</c> or
///     <c>zh-Hans-CN</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             The tag must name a predefined culture (<see cref="CultureInfo.GetCultureInfo(string, bool)" />), so well-formed tags .NET has
///             no data for (<c>tlh</c>, <c>de-CH-1996</c>), private use tags (<c>x-private</c>) and the invariant culture (the empty string) are
///             rejected.
///         </item>
///         <item>
///             BCP 47 tags are case-insensitive, with a conventional casing (RFC 5646 §2.1.1). SuperStrong validates without changing the
///             value, so the tag must be written as .NET names the culture (<c>en-US</c>, not <c>EN-us</c> or <c>en_US</c>).
///         </item>
///         <item>
///             What .NET knows comes from the platform's ICU data, so the accepted tags can differ between platforms; in invariant
///             globalization mode (<c>InvariantGlobalization</c>) every tag is rejected.
///         </item>
///     </list>
/// </remarks>
public sealed class LanguageCodeRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .Satisfies(IsKnownTag, "Value must be a BCP 47 language tag of a culture .NET knows, in its canonical case.");

    private static bool IsKnownTag(string value)
    {
        if (value.Length == 0) return false;
        try
        {
            return string.Equals(CultureInfo.GetCultureInfo(value, predefinedOnly: true).Name, value, StringComparison.Ordinal);
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    private LanguageCodeRules() { }
}

/// <summary>A BCP 47 language tag, valid under <see cref="LanguageCodeRules" />.</summary>
[StrongType<string, LanguageCodeRules>]
public sealed partial class LanguageCode;
