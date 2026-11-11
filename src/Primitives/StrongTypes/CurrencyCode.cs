using System.Text.RegularExpressions;

using Rocket.Surgery.LaunchPad.StrongTypes.Registries;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a currency code: an ISO 4217 alphabetic code in current use, such as <c>USD</c> or <c>EUR</c>.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             The code must be on ISO 4217 list one (current currencies and funds), a list kept in this package as published on 2026-09-17.
///             It includes the funds codes (<c>USN</c>, <c>CLF</c>), precious metals (<c>XAU</c>), <c>XTS</c> (testing) and <c>XXX</c> (no
///             currency); withdrawn codes (<c>HRK</c>, <c>BGN</c>, <c>ANG</c>) are rejected.
///         </item>
///         <item>
///             ISO 4217 writes codes in upper case. SuperStrong validates without changing the value, so lower case (<c>usd</c>) is rejected
///             rather than converted.
///         </item>
///     </list>
/// </remarks>
public sealed partial class CurrencyCodeRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .MatchesRegex(Pattern())
                                                         .Satisfies(Iso4217Currencies.Codes.Contains, "Value must be a current ISO 4217 currency code.");

    [GeneratedRegex(@"^[A-Z]{3}\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private CurrencyCodeRules() { }
}

/// <summary>An ISO 4217 currency code, valid under <see cref="CurrencyCodeRules" />.</summary>
[StrongType<string, CurrencyCodeRules>]
public sealed partial class CurrencyCode;
