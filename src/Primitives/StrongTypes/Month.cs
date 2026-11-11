using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a month of the year.
/// </summary>
/// <remarks>
///     1 (January) to 12 (December), as ISO 8601 numbers them.
/// </remarks>
public sealed class MonthRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(1).HasMaxValue(12);

    private MonthRules() { }
}

/// <summary>A month of the year, valid under <see cref="MonthRules" />.</summary>
[StrongType<int, MonthRules>]
public sealed partial class Month;
