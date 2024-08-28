using NodaTime;
using NodaTime.Text;
using Riok.Mapperly.Abstractions;

namespace Rocket.Surgery.LaunchPad.Mapping.Profiles;

/// <summary>
///     A mapper used to map between NodaTime and <see cref="DateTimeOffset" />, <see cref="TimeOnly" />, <see cref="DateOnly" /> and <see cref="TimeSpan" />
/// </summary>
[Mapper]
[PublicAPI]
public static partial class NodaTimeMapper
{
    /// <summary>
    ///     Converts a <see cref="TimeSpan" /> to a <see cref="Duration" />.
    /// </summary>
    /// <param name="source">The <see cref="TimeSpan" /> to convert.</param>
    /// <returns>The equivalent <see cref="Duration" />.</returns>
    public static Duration ToDuration(TimeSpan source)
    {
        return Duration.FromTimeSpan(source);
    }

    /// <summary>
    ///     Converts a <see cref="Duration" /> to a <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="source">The <see cref="Duration" /> to convert.</param>
    /// <returns>The equivalent <see cref="TimeSpan" />.</returns>
    public static TimeSpan FromDuration(Duration source)
    {
        return source.ToTimeSpan();
    }

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> to a <see cref="Instant" />.
    /// </summary>
    /// <param name="source">The <see cref="DateTimeOffset" /> to convert.</param>
    /// <returns>The equivalent <see cref="Instant" />.</returns>
    public static Instant ToInstant(DateTimeOffset source)
    {
        return Instant.FromDateTimeOffset(source);
    }

    /// <summary>
    ///     Converts a <see cref="Instant" /> to a <see cref="DateTimeOffset" />.
    /// </summary>
    /// <param name="source">The <see cref="Instant" /> to convert.</param>
    /// <returns>The equivalent <see cref="DateTimeOffset" />.</returns>
    public static DateTimeOffset FromInstant(Instant source)
    {
        return source.ToDateTimeOffset();
    }

    /// <summary>
    ///     Converts a <see cref="DateOnly" /> to a <see cref="LocalDate" />.
    /// </summary>
    /// <param name="source">The <see cref="DateOnly" /> to convert.</param>
    /// <returns>The equivalent <see cref="LocalDate" />.</returns>
    public static LocalDate ToLocalDate(DateOnly source)
    {
        return LocalDate.FromDateOnly(source);
    }

    /// <summary>
    ///     Converts a <see cref="LocalDate" /> to a <see cref="DateOnly" />.
    /// </summary>
    /// <param name="source">The <see cref="LocalDate" /> to convert.</param>
    /// <returns>The equivalent <see cref="DateOnly" />.</returns>
    public static DateOnly FromLocalDate(LocalDate source)
    {
        return source.ToDateOnly();
    }

    /// <summary>
    ///     Converts a <see cref="TimeOnly" /> to a <see cref="LocalTime" />.
    /// </summary>
    /// <param name="source">The <see cref="TimeOnly" /> to convert.</param>
    /// <returns>The equivalent <see cref="LocalTime" />.</returns>
    public static LocalTime ToLocalTime(TimeOnly source)
    {
        return LocalTime.FromTimeOnly(source);
    }

    /// <summary>
    ///     Converts a <see cref="LocalTime" /> to a <see cref="TimeOnly" />.
    /// </summary>
    /// <param name="source">The <see cref="LocalTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="TimeOnly" />.</returns>
    public static TimeOnly FromLocalTime(LocalTime source)
    {
        return source.ToTimeOnly();
    }

    /// <summary>
    ///     Converts a <see cref="TimeSpan" /> to a <see cref="Offset" />.
    /// </summary>
    /// <param name="source">The <see cref="TimeSpan" /> to convert.</param>
    /// <returns>The equivalent <see cref="Offset" />.</returns>
    public static Offset ToOffset(TimeSpan source)
    {
        return Offset.FromTicks(source.Ticks);
    }

    /// <summary>
    ///     Converts a <see cref="Offset" /> to a <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="source">The <see cref="Offset" /> to convert.</param>
    /// <returns>The equivalent <see cref="TimeSpan" />.</returns>
    public static TimeSpan FromOffset(Offset source)
    {
        return source.ToTimeSpan();
    }

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> to a <see cref="OffsetDateTime" />.
    /// </summary>
    /// <param name="source">The <see cref="DateTimeOffset" /> to convert.</param>
    /// <returns>The equivalent <see cref="OffsetDateTime" />.</returns>
    public static OffsetDateTime ToOffsetDateTime(DateTimeOffset source)
    {
        return OffsetDateTime.FromDateTimeOffset(source);
    }

    /// <summary>
    ///     Converts a <see cref="OffsetDateTime" /> to a <see cref="DateTimeOffset" />.
    /// </summary>
    /// <param name="source">The <see cref="OffsetDateTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="DateTimeOffset" />.</returns>
    public static DateTimeOffset FromOffsetDateTime(OffsetDateTime source)
    {
        return source.ToDateTimeOffset();
    }

    /// <summary>
    ///     Converts a <see cref="string" /> to a <see cref="Period" />.
    /// </summary>
    /// <param name="source">The <see cref="string" /> to convert.</param>
    /// <returns>The equivalent <see cref="Period" />.</returns>
    public static Period ToPeriod(string source)
    {
        return PeriodPattern.Roundtrip.Parse(source).Value;
    }

    /// <summary>
    ///     Converts a <see cref="Period" /> to a <see cref="string" />.
    /// </summary>
    /// <param name="source">The <see cref="Period" /> to convert.</param>
    /// <returns>The equivalent <see cref="string" />.</returns>
    public static string FromPeriod(Period source)
    {
        return source.ToString();
    }
}