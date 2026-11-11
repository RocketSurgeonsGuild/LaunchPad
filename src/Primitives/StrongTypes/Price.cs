using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a price: an amount of money with no currency, such as a unit price.
/// </summary>
/// <remarks>
///     Zero or more. A price is not rounded to a currency's minor unit; pair it with a <see cref="CurrencyCode" /> or use <see cref="Money" /> when the currency matters.
/// </remarks>
public sealed class PriceRules : IStrongTypeTemplate<decimal>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<decimal> Define() => StrongType.Define<decimal>().IsPositiveOrZero();

    private PriceRules() { }
}

/// <summary>A non-negative price, valid under <see cref="PriceRules" />.</summary>
[StrongType<decimal, PriceRules>]
public sealed partial class Price;
