using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a percentage.
/// </summary>
/// <remarks>
///     0 to 100 inclusive, as a number of hundredths: 12.5 is 12.5 %, not 0.125.
/// </remarks>
public sealed class PercentageRules : IStrongTypeTemplate<decimal>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<decimal> Define() => StrongType.Define<decimal>().HasMinValue(0m).HasMaxValue(100m);

    private PercentageRules() { }
}

/// <summary>A percentage from 0 to 100, valid under <see cref="PercentageRules" />.</summary>
[StrongType<decimal, PercentageRules>]
public sealed partial class Percentage;
