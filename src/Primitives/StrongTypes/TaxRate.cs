using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a tax rate, as a percentage of the taxed amount.
/// </summary>
/// <remarks>
///     0 to 100 inclusive, as a number of hundredths: 20 is 20 %. No tax uses more than 100 %, and none is negative.
/// </remarks>
public sealed class TaxRateRules : IStrongTypeTemplate<decimal>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<decimal> Define() => StrongType.Define<decimal>().HasMinValue(0m).HasMaxValue(100m);

    private TaxRateRules() { }
}

/// <summary>A tax rate as a percentage, valid under <see cref="TaxRateRules" />.</summary>
[StrongType<decimal, TaxRateRules>]
public sealed partial class TaxRate;
