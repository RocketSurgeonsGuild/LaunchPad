using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     An amount of money in a currency.
/// </summary>
/// <remarks>
///     <para>
///         The amount can be any <see langword="decimal" />, including negative amounts (a refund, a balance owed), and is not rounded to the
///         currency's minor unit, so unit prices with more decimals fit. The currency is required.
///     </para>
///     <para>
///         Like a strong type, it is valid by construction: <see cref="From" /> and the constructor throw without a currency, and
///         <see cref="TryFrom" /> returns <see langword="false" />. It serializes as a JSON object with <c>amount</c> and <c>currency</c>
///         members, the currency as its ISO 4217 code.
///     </para>
/// </remarks>
public sealed record Money
{
    /// <summary>Creates an amount of money.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="currency" /> is <see langword="null" />.</exception>
    [JsonConstructor]
    public Money(decimal amount, CurrencyCode currency)
    {
        ArgumentNullException.ThrowIfNull(currency);
        Amount = amount;
        Currency = currency;
    }

    /// <summary>Creates an amount of money.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="currency" /> is <see langword="null" />.</exception>
    public static Money From(decimal amount, CurrencyCode currency) => new(amount, currency);

    /// <summary>Creates an amount of money, or returns <see langword="false" /> without a currency.</summary>
    public static bool TryFrom(decimal amount, CurrencyCode? currency, [NotNullWhen(true)] out Money? result)
    {
        result = currency is null ? null : new(amount, currency);
        return result is not null;
    }

    /// <summary>The amount, in units of the currency (not minor units: 12.50 US dollars is 12.50).</summary>
    public decimal Amount { get; private init; }

    /// <summary>The currency.</summary>
    public CurrencyCode Currency { get; private init; }

    /// <inheritdoc />
    public override string ToString() => string.Create(System.Globalization.CultureInfo.InvariantCulture, $"{Amount} {Currency}");
}
