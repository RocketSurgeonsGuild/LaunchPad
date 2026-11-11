using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     A range of times of day from <see cref="Start" /> to <see cref="End" /> within one day. <see cref="Start" /> is never after
///     <see cref="End" />, so a range cannot run past midnight; an empty range has them equal.
/// </summary>
/// <remarks>
///     Like a strong type, it is valid by construction: <see cref="From" /> and the constructor throw for an invalid range, and
///     <see cref="TryFrom" /> returns <see langword="false" />. It serializes as a JSON object with <c>start</c> and <c>end</c> members, and
///     reading one checks it the same way.
/// </remarks>
public sealed record TimeRange
{
    /// <summary>Creates a range from <paramref name="start" /> to <paramref name="end" />.</summary>
    /// <exception cref="ArgumentException"><paramref name="start" /> is after <paramref name="end" />.</exception>
    [JsonConstructor]
    public TimeRange(TimeOnly start, TimeOnly end)
    {
        if (start > end) throw new ArgumentException($"The start ({start:O}) must not be after the end ({end:O}).", nameof(start));
        Start = start;
        End = end;
    }

    /// <summary>Creates a range from <paramref name="start" /> to <paramref name="end" />.</summary>
    /// <exception cref="ArgumentException"><paramref name="start" /> is after <paramref name="end" />.</exception>
    public static TimeRange From(TimeOnly start, TimeOnly end) => new(start, end);

    /// <summary>Creates a range from <paramref name="start" /> to <paramref name="end" />, or returns <see langword="false" /> if it is invalid.</summary>
    public static bool TryFrom(TimeOnly start, TimeOnly end, [NotNullWhen(true)] out TimeRange? result)
    {
        result = start <= end ? new(start, end) : null;
        return result is not null;
    }

    /// <summary>The time the range starts at.</summary>
    public TimeOnly Start { get; }

    /// <summary>The time the range ends at.</summary>
    public TimeOnly End { get; }

    /// <summary>How long the range is.</summary>
    [JsonIgnore]
    public TimeSpan Duration => End - Start;
}
