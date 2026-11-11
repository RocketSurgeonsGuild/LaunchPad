using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a latitude in decimal degrees (WGS 84, ISO 6709).
/// </summary>
/// <remarks>
///     A finite number from -90 (the South Pole) to 90 (the North Pole) inclusive. NaN and infinities are rejected.
/// </remarks>
public sealed class LatitudeRules : IStrongTypeTemplate<double>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<double> Define() => StrongType.Define<double>().Satisfies(double.IsFinite, "Value must be a finite number.").HasMinValue(-90d).HasMaxValue(90d);

    private LatitudeRules() { }
}

/// <summary>A latitude in decimal degrees, valid under <see cref="LatitudeRules" />.</summary>
[StrongType<double, LatitudeRules>]
public sealed partial class Latitude;
