using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     A value generator that generates a <see cref="DateTimeOffset" /> from the NodaTime <see cref="IClock" />
/// </summary>
/// <param name="clock">The clock that the values are read from.</param>
[PublicAPI]
public class NodaTimeDateTimeOffsetValueGenerator(IClock clock) : ValueGenerator<DateTimeOffset>
{
    /// <inheritdoc />
    public override bool GeneratesTemporaryValues { get; }

    /// <inheritdoc />
    public override DateTimeOffset Next(EntityEntry entry) => clock.GetCurrentInstant().ToDateTimeOffset();
}
