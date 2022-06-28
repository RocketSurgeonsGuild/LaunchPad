using AutoMapper;
using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.Mapping.Profiles;

/// <summary>
///     NodaTimeProfile.
///     Implements the <see cref="Profile" />
/// </summary>
/// <seealso cref="Profile" />
public class NodaTimeProfile : Profile
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NodaTimeProfile" /> class.
    /// </summary>
    public NodaTimeProfile()
    {
        CreateMappingsForDurationConverter();
        CreateMappingsForInstantConvertor();
        CreateMappingsForLocalDateConverter();
        CreateMappingsForLocalDateTimeConverter();
        CreateMappingsForLocalTimeConverter();
        CreateMappingsForOffsetConverter();
        CreateMappingsForOffsetDateTimeConverter();
        CreateMappingsForPeriodConverter();
    }

    /// <summary>
    ///     Gets the name of the profile.
    /// </summary>
    /// <value>The name of the profile.</value>
    public override string ProfileName => nameof(NodaTimeProfile);

    private void CreateMappingsForDurationConverter()
    {
        CreateMap<decimal, Duration>().ConvertUsing(source => Duration.FromTicks((long)( source * NodaConstants.TicksPerMillisecond )));
        CreateMap<double, Duration>().ConvertUsing(source => Duration.FromTicks((long)( source * NodaConstants.TicksPerMillisecond )));
        CreateMap<Duration, decimal>().ConvertUsing(source => (decimal)source.BclCompatibleTicks / NodaConstants.TicksPerMillisecond);
        CreateMap<Duration, double>().ConvertUsing(source => (double)source.BclCompatibleTicks / NodaConstants.TicksPerMillisecond);
        CreateMap<Duration, int>().ConvertUsing(source => (int)( source.BclCompatibleTicks / NodaConstants.TicksPerSecond ));
        CreateMap<Duration, long>().ConvertUsing(source => source.BclCompatibleTicks / NodaConstants.TicksPerMillisecond);
        CreateMap<Duration, TimeSpan>().ConvertUsing(source => source.ToTimeSpan());
        CreateMap<int, Duration>().ConvertUsing(source => Duration.FromTicks(source * NodaConstants.TicksPerSecond));
        CreateMap<long, Duration>().ConvertUsing(source => Duration.FromTicks(source * NodaConstants.TicksPerMillisecond));
        CreateMap<TimeSpan, Duration>().ConvertUsing(source => Duration.FromTimeSpan(source));
    }

    private void CreateMappingsForInstantConvertor()
    {
        CreateMap<DateTime, Instant>().ConvertUsing(
            source => Instant.FromDateTimeUtc(
                source.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(source, DateTimeKind.Utc) : source.ToUniversalTime()
            )
        );
        CreateMap<DateTimeOffset, Instant>().ConvertUsing(source => Instant.FromDateTimeOffset(source));
        CreateMap<Instant, DateTime>().ConvertUsing(source => source.ToDateTimeUtc());
        CreateMap<Instant, DateTimeOffset>().ConvertUsing(source => source.ToDateTimeOffset());
    }

    private void CreateMappingsForLocalDateConverter()
    {
        CreateMap<DateTime, LocalDate>().ConvertUsing(source => LocalDate.FromDateTime(source));
        CreateMap<LocalDate, DateTime>().ConvertUsing(source => source.AtMidnight().ToDateTimeUnspecified());
#if NET6_0_OR_GREATER
        CreateMap<DateOnly, LocalDate>().ConvertUsing(source => LocalDate.FromDateOnly(source));
        CreateMap<LocalDate, DateOnly>().ConvertUsing(source => source.ToDateOnly());
#endif
    }

    private void CreateMappingsForLocalDateTimeConverter()
    {
        CreateMap<DateTime, LocalDateTime>().ConvertUsing(source => LocalDateTime.FromDateTime(source));
        CreateMap<LocalDateTime, DateTime>().ConvertUsing(source => source.ToDateTimeUnspecified());
    }

    private void CreateMappingsForLocalTimeConverter()
    {
#if NET6_0_OR_GREATER
        CreateMap<TimeOnly, LocalTime>().ConvertUsing(source => LocalTime.FromTimeOnly(source));
        CreateMap<LocalTime, TimeOnly>().ConvertUsing(source => source.ToTimeOnly());
#endif
        CreateMap<LocalTime, TimeSpan>().ConvertUsing(source => new TimeSpan(source.TickOfDay));
        CreateMap<TimeSpan, LocalTime>().ConvertUsing(source => LocalTime.FromTicksSinceMidnight(source.Ticks));
    }

    private void CreateMappingsForOffsetConverter()
    {
        CreateMap<Offset, TimeSpan>().ConvertUsing(source => source.ToTimeSpan());
        CreateMap<TimeSpan, Offset>().ConvertUsing(source => Offset.FromTicks(source.Ticks));
    }

    private void CreateMappingsForOffsetDateTimeConverter()
    {
        CreateMap<DateTimeOffset, OffsetDateTime>().ConvertUsing(source => OffsetDateTime.FromDateTimeOffset(source));
        CreateMap<OffsetDateTime, DateTimeOffset>().ConvertUsing(source => source.ToDateTimeOffset());
    }

    private void CreateMappingsForPeriodConverter()
    {
        CreateMap<Period?, string?>().ConvertUsing(source => source == default ? default : source.ToString());
        CreateMap<string?, Period?>().ConvertUsing(source => ( source == default ? default : PeriodPattern.Roundtrip.Parse(source).Value ) ?? default(Period?));
    }
}
