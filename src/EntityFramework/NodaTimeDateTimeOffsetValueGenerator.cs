using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     A value generator for DateTimeOffset from the NodaTime <see cref="IClock" />
/// </summary>
[PublicAPI]
public class NodaTimeDateTimeOffsetValueGenerator : ValueGenerator<DateTimeOffset>
{
    private readonly IClock _clock;

    /// <summary>
    ///     Create the generator with the given clock
    /// </summary>
    /// <param name="clock"></param>
    public NodaTimeDateTimeOffsetValueGenerator(IClock clock)
    {
        _clock = clock;
    }

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues { get; }

    /// <inheritdoc />
    public override DateTimeOffset Next(EntityEntry entry)
    {
        return _clock.GetCurrentInstant().ToDateTimeOffset();
    }
}
