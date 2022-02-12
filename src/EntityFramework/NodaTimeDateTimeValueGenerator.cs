using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     A value generator for DateTimeOffset from the NodaTime <see cref="IClock" />
/// </summary>
[PublicAPI]
public class NodaTimeDateTimeValueGenerator : ValueGenerator<DateTime>
{
    private readonly IClock _clock;

    /// <summary>
    ///     Create the generator with the given clock
    /// </summary>
    /// <param name="clock"></param>
    public NodaTimeDateTimeValueGenerator(IClock clock)
    {
        _clock = clock;
    }

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues { get; }

    /// <inheritdoc />
    public override DateTime Next(EntityEntry entry)
    {
        return _clock.GetCurrentInstant().ToDateTimeUtc();
    }
}
