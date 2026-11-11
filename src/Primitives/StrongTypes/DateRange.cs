using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     A range of dates from <see cref="Start" /> to <see cref="End" />, both included. <see cref="Start" /> is never after
///     <see cref="End" />; a range of one day has them equal.
/// </summary>
/// <remarks>
///     Like a strong type, it is valid by construction: <see cref="From" /> and the constructor throw for an invalid range, and
///     <see cref="TryFrom" /> returns <see langword="false" />. It serializes as a JSON object with <c>start</c> and <c>end</c> members, and
///     reading one checks it the same way.
/// </remarks>
public sealed record DateRange
{
    /// <summary>Creates a range from <paramref name="start" /> to <paramref name="end" />.</summary>
    /// <exception cref="ArgumentException"><paramref name="start" /> is after <paramref name="end" />.</exception>
    [JsonConstructor]
    public DateRange(DateOnly start, DateOnly end)
    {
        if (start > end) throw new ArgumentException($"The start ({start:O}) must not be after the end ({end:O}).", nameof(start));
        Start = start;
        End = end;
    }

    /// <summary>Creates a range from <paramref name="start" /> to <paramref name="end" />.</summary>
    /// <exception cref="ArgumentException"><paramref name="start" /> is after <paramref name="end" />.</exception>
    public static DateRange From(DateOnly start, DateOnly end) => new(start, end);

    /// <summary>Creates a range from <paramref name="start" /> to <paramref name="end" />, or returns <see langword="false" /> if it is invalid.</summary>
    public static bool TryFrom(DateOnly start, DateOnly end, [NotNullWhen(true)] out DateRange? result)
    {
        result = start <= end ? new(start, end) : null;
        return result is not null;
    }

    /// <summary>The first day of the range.</summary>
    public DateOnly Start { get; private init; }

    /// <summary>The last day of the range.</summary>
    public DateOnly End { get; private init; }

    /// <summary>Whether <paramref name="date" /> is in the range.</summary>
    public bool Contains(DateOnly date) => date >= Start && date <= End;
}
