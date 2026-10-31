using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     A value generator that generates a UTC <see cref="DateTime" /> from the NodaTime <see cref="IClock" />
/// </summary>
/// <param name="clock">The clock that the values are read from.</param>
[PublicAPI]
public class NodaTimeDateTimeValueGenerator(IClock clock) : ValueGenerator<DateTime>
{
    /// <inheritdoc />
    public override bool GeneratesTemporaryValues { get; }

    /// <inheritdoc />
    public override DateTime Next(EntityEntry entry) => clock.GetCurrentInstant().ToDateTimeUtc();
}
