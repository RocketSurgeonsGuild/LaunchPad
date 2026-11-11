using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a longitude in decimal degrees (WGS 84, ISO 6709).
/// </summary>
/// <remarks>
///     A finite number from -180 to 180 inclusive; both ends name the antimeridian, as ISO 6709 and GeoJSON allow. NaN and infinities are rejected.
/// </remarks>
public sealed class LongitudeRules : IStrongTypeTemplate<double>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<double> Define() => StrongType.Define<double>().Satisfies(double.IsFinite, "Value must be a finite number.").HasMinValue(-180d).HasMaxValue(180d);

    private LongitudeRules() { }
}

/// <summary>A longitude in decimal degrees, valid under <see cref="LongitudeRules" />.</summary>
[StrongType<double, LongitudeRules>]
public sealed partial class Longitude;
