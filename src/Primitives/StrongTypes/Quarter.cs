using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a quarter of the year.
/// </summary>
/// <remarks>
///     1 to 4: the first quarter is January to March.
/// </remarks>
public sealed class QuarterRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(1).HasMaxValue(4);

    private QuarterRules() { }
}

/// <summary>A quarter of the year, valid under <see cref="QuarterRules" />.</summary>
[StrongType<int, QuarterRules>]
public sealed partial class Quarter;
