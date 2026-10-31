using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     A value generator for DateTimeOffset from the NodaTime <see cref="IClock" />
/// </summary>
/// <remarks>
///     Create the generator with the given clock
/// </remarks>
/// <param name="clock"></param>
[PublicAPI]
public class NodaTimeDateTimeOffsetValueGenerator(IClock clock) : ValueGenerator<DateTimeOffset>
{
    private readonly IClock _clock = clock;

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues { get; }

    /// <inheritdoc />
    public override DateTimeOffset Next(EntityEntry entry) => _clock.GetCurrentInstant().ToDateTimeOffset();
}
