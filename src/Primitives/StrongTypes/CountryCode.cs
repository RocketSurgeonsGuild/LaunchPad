using System.Text.RegularExpressions;

using Rocket.Surgery.LaunchPad.StrongTypes.Registries;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a country code: an officially assigned ISO 3166-1 alpha-2 code, such as <c>US</c> or <c>DE</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             The code must be one of the 249 officially assigned codes, a list kept in this package. User-assigned codes (<c>XK</c> for
///             Kosovo, <c>ZZ</c>), exceptionally reserved codes (<c>UK</c>, <c>EU</c>) and withdrawn codes (<c>AN</c>, <c>CS</c>) are rejected.
///         </item>
///         <item>
///             ISO 3166-1 writes codes in upper case. SuperStrong validates without changing the value, so lower case (<c>us</c>) is rejected
///             rather than converted.
///         </item>
///         <item>
///             The list is not read from <see cref="System.Globalization.RegionInfo" />: it accepts user-assigned and withdrawn codes, misses
///             some assigned ones, and depends on the platform's ICU data and on invariant globalization mode.
///         </item>
///     </list>
/// </remarks>
public sealed partial class CountryCodeRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .MatchesRegex(Pattern())
                                                         .Satisfies(
                                                              Iso3166Countries.Codes.Contains,
                                                              "Value must be an officially assigned ISO 3166-1 alpha-2 code."
                                                          );

    [GeneratedRegex(@"^[A-Z]{2}\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private CountryCodeRules() { }
}

/// <summary>An ISO 3166-1 alpha-2 country code, valid under <see cref="CountryCodeRules" />.</summary>
[StrongType<string, CountryCodeRules>]
public sealed partial class CountryCode;
