using NodaTime;
using Riok.Mapperly.Abstractions;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     A mapper used to map between NodaTime and <see cref="DateTime" />
/// </summary>
/// <remarks>
///     If you're using this type... you know what you're doing or you're doing it wrong.
/// </remarks>
[Mapper]
[PublicAPI]
public partial class NodaTimeDateTimeMapper
{
    /// <summary>
    ///     Converts a <see cref="DateTime" /> to a <see cref="Instant" />.
    /// </summary>
    /// <param name="source">The <see cref="DateTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="Instant" />.</returns>
    public static Instant ToInstant(DateTime source)
    {
        return Instant.FromDateTimeUtc(
            source.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(source, DateTimeKind.Utc)
                : source.ToUniversalTime()
        );
    }

    /// <summary>
    ///     Converts a <see cref="Instant" /> to a <see cref="DateTime" />.
    /// </summary>
    /// <param name="source">The <see cref="Instant" /> to convert.</param>
    /// <returns>The equivalent <see cref="DateTime" />.</returns>
    public static DateTime FromInstant(Instant source)
    {
        return source.ToDateTimeUtc();
    }

    /// <summary>
    ///     Converts a <see cref="DateTime" /> to a <see cref="LocalDate" />.
    /// </summary>
    /// <param name="source">The <see cref="DateTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="LocalDate" />.</returns>
    public static LocalDate ToLocalDate(DateTime source)
    {
        return LocalDate.FromDateTime(source);
    }

    /// <summary>
    ///     Converts a <see cref="LocalDate" /> to a <see cref="DateTime" />.
    /// </summary>
    /// <param name="source">The <see cref="LocalDate" /> to convert.</param>
    /// <returns>The equivalent <see cref="DateTime" />.</returns>
    public static DateTime FromLocalDate(LocalDate source)
    {
        return source.AtMidnight().ToDateTimeUnspecified();
    }

    /// <summary>
    ///     Converts a <see cref="DateTime" /> to a <see cref="LocalDateTime" />.
    /// </summary>
    /// <param name="source">The <see cref="DateTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="LocalDateTime" />.</returns>
    public static LocalDateTime ToLocalDateTime(DateTime source)
    {
        return LocalDateTime.FromDateTime(source);
    }

    /// <summary>
    ///     Converts a <see cref="LocalDateTime" /> to a <see cref="DateTime" />.
    /// </summary>
    /// <param name="source">The <see cref="LocalDateTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="DateTime" />.</returns>
    public static DateTime FromLocalDateTime(LocalDateTime source)
    {
        return source.ToDateTimeUnspecified();
    }

    /// <summary>
    ///     Converts a <see cref="TimeSpan" /> to a <see cref="LocalTime" />.
    /// </summary>
    /// <param name="source">The <see cref="TimeSpan" /> to convert.</param>
    /// <returns>The equivalent <see cref="LocalTime" />.</returns>
    public static LocalTime ToLocalTime(TimeSpan source)
    {
        return LocalTime.FromTicksSinceMidnight(source.Ticks);
    }

    /// <summary>
    ///     Converts a <see cref="LocalTime" /> to a <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="source">The <see cref="LocalTime" /> to convert.</param>
    /// <returns>The equivalent <see cref="TimeSpan" />.</returns>
    public static TimeSpan FromLocalTime(LocalTime source)
    {
        return new(source.TickOfDay);
    }
}