using System.Globalization;
using System.Text;

using NodaTime;

using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose Duration Serializer
/// </summary>
/// <remarks>
/// Reads and writes the ISO 8601 durations of the <c>Duration</c> scalar of HotChocolate 16: <c>[-]P[nW][nD][T[nH][nM][n[.f]S]]</c>, with an
/// optional sign on each component. Years and months have no fixed length, so a duration with them is rejected.
/// </remarks>
public class DurationSerializer : ScalarSerializer<string, Duration>
{
    /// <summary>
    /// Create a new DurationSerializer
    /// </summary>
    public DurationSerializer() : base("Duration")
    {
    }

    /// <inheritdoc />
    public override Duration Parse(string serializedValue) =>
        TryParse(serializedValue, out var value)
            ? value
            : throw new FormatException($"The value \"{serializedValue}\" is not an ISO 8601 duration.");

    /// <inheritdoc />
    protected override string Format(Duration runtimeValue)
    {
        if (runtimeValue == Duration.Zero) return "PT0S";

        var builder = new StringBuilder(32);
        if (runtimeValue < Duration.Zero) builder.Append('-');
        builder.Append('P');

        var days = Math.Abs((long)runtimeValue.Days);
        var hours = Math.Abs(runtimeValue.Hours);
        var minutes = Math.Abs(runtimeValue.Minutes);
        var seconds = Math.Abs(runtimeValue.Seconds);
        var nanoseconds = Math.Abs(runtimeValue.SubsecondNanoseconds);

        if (days != 0) builder.Append(days.ToString(CultureInfo.InvariantCulture)).Append('D');
        if (hours == 0 && minutes == 0 && seconds == 0 && nanoseconds == 0) return builder.ToString();

        builder.Append('T');
        if (hours != 0) builder.Append(hours.ToString(CultureInfo.InvariantCulture)).Append('H');
        if (minutes != 0) builder.Append(minutes.ToString(CultureInfo.InvariantCulture)).Append('M');
        if (seconds != 0 || nanoseconds != 0)
        {
            builder.Append(seconds.ToString(CultureInfo.InvariantCulture));
            if (nanoseconds != 0) builder.Append('.').Append(nanoseconds.ToString("D9", CultureInfo.InvariantCulture).TrimEnd('0'));
            builder.Append('S');
        }

        return builder.ToString();
    }

    private static bool TryParse(string value, out Duration duration)
    {
        duration = Duration.Zero;
        var span = value.AsSpan();
        var negative = span is ['-', ..];
        if (negative) span = span[1..];
        if (span is not ['P', _, ..]) return false;
        span = span[1..];

        var time = false;
        var order = 0;
        var total = Duration.Zero;
        while (!span.IsEmpty)
        {
            if (span[0] == 'T')
            {
                if (time || span.Length == 1) return false;
                time = true;
                span = span[1..];
                continue;
            }

            var sign = 1;
            if (span[0] == '-')
            {
                sign = -1;
                span = span[1..];
            }

            var length = 0;
            while (length < span.Length && ( char.IsAsciiDigit(span[length]) || span[length] is '.' or ',' )) length++;
            if (length == 0 || length == span.Length) return false;

            var number = span[..length].ToString().Replace(',', '.');
            var designator = span[length];
            span = span[( length + 1 )..];

            var (rank, unit) = (time, designator) switch
            {
                (false, 'W') => (1, Duration.FromDays(7)),
                (false, 'D') => (2, Duration.FromDays(1)),
                (true, 'H') => (3, Duration.FromHours(1)),
                (true, 'M') => (4, Duration.FromMinutes(1)),
                (true, 'S') => (5, Duration.FromSeconds(1)),
                _ => (0, Duration.Zero),
            };
            if (rank <= order) return false;
            order = rank;

            if (designator == 'S')
            {
                if (!decimal.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var secondsValue)) return false;
                var wholeSeconds = decimal.Truncate(secondsValue);
                var nanoseconds = (long)decimal.Truncate(( secondsValue - wholeSeconds ) * 1_000_000_000m);
                total += sign * ( Duration.FromSeconds((long)wholeSeconds) + Duration.FromNanoseconds(nanoseconds) );
            }
            else
            {
                if (!long.TryParse(number, NumberStyles.None, CultureInfo.InvariantCulture, out var count)) return false;
                total += sign * count * unit;
            }
        }

        duration = negative ? -total : total;
        return true;
    }
}
