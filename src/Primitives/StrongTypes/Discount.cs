using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a discount, as a percentage taken off a price.
/// </summary>
/// <remarks>
///     0 to 100 inclusive, as a number of hundredths: 15 is 15 % off. A fixed-amount discount is a <see cref="Price" /> or <see cref="Money" />.
/// </remarks>
public sealed class DiscountRules : IStrongTypeTemplate<decimal>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<decimal> Define() => StrongType.Define<decimal>().HasMinValue(0m).HasMaxValue(100m);

    private DiscountRules() { }
}

/// <summary>A percentage discount, valid under <see cref="DiscountRules" />.</summary>
[StrongType<decimal, DiscountRules>]
public sealed partial class Discount;
