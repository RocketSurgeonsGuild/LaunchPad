using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a fiscal year, named by a calendar year (the year it ends in or starts in, as the organization chooses).
/// </summary>
/// <remarks>
///     1 to 9999, the years a <see cref="DateOnly" /> can represent. Which calendar year names the fiscal year is the application's convention.
/// </remarks>
public sealed class FiscalYearRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(1).HasMaxValue(9999);

    private FiscalYearRules() { }
}

/// <summary>A fiscal year, valid under <see cref="FiscalYearRules" />.</summary>
[StrongType<int, FiscalYearRules>]
public sealed partial class FiscalYear;
