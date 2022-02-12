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
        CreateMap<decimal?, Duration?>().ConvertUsing(
            source => source.HasValue ? Duration.FromTicks((long)( source.Value * NodaConstants.TicksPerMillisecond )) : default(Duration?)
        );
        CreateMap<double, Duration>().ConvertUsing(source => Duration.FromTicks((long)( source * NodaConstants.TicksPerMillisecond )));
        CreateMap<double?, Duration?>().ConvertUsing(
            source => source.HasValue ? Duration.FromTicks((long)( source.Value * NodaConstants.TicksPerMillisecond )) : default(Duration?)
        );
        CreateMap<Duration, decimal>().ConvertUsing(source => (decimal)source.BclCompatibleTicks / NodaConstants.TicksPerMillisecond);
        CreateMap<Duration, double>().ConvertUsing(source => (double)source.BclCompatibleTicks / NodaConstants.TicksPerMillisecond);
        CreateMap<Duration, int>().ConvertUsing(source => (int)( source.BclCompatibleTicks / NodaConstants.TicksPerSecond ));
        CreateMap<Duration, long>().ConvertUsing(source => source.BclCompatibleTicks / NodaConstants.TicksPerMillisecond);
        CreateMap<Duration, TimeSpan>().ConvertUsing(source => source.ToTimeSpan());
        CreateMap<Duration?, decimal?>().ConvertUsing(
            source => source.HasValue ? (decimal)source.Value.BclCompatibleTicks / NodaConstants.TicksPerMillisecond : default(decimal?)
        );
        CreateMap<Duration?, double?>().ConvertUsing(
            source => source.HasValue ? (double)source.Value.BclCompatibleTicks / NodaConstants.TicksPerMillisecond : default(double?)
        );
        CreateMap<Duration?, int?>()
           .ConvertUsing(source => source.HasValue ? (int)( source.Value.BclCompatibleTicks / NodaConstants.TicksPerSecond ) : default(int?));
        CreateMap<Duration?, long?>()
           .ConvertUsing(source => source.HasValue ? source.Value.BclCompatibleTicks / NodaConstants.TicksPerMillisecond : default(long?));
        CreateMap<Duration?, TimeSpan?>().ConvertUsing(source => source.HasValue ? source.Value.ToTimeSpan() : default(TimeSpan?));
        CreateMap<int, Duration>().ConvertUsing(source => Duration.FromTicks(source * NodaConstants.TicksPerSecond));
        CreateMap<int?, Duration?>()
           .ConvertUsing(source => source.HasValue ? Duration.FromTicks(source.Value * NodaConstants.TicksPerSecond) : default(Duration?));
        CreateMap<long, Duration>().ConvertUsing(source => Duration.FromTicks(source * NodaConstants.TicksPerMillisecond));
        CreateMap<long?, Duration?>().ConvertUsing(
            source => source.HasValue ? Duration.FromTicks(source.Value * NodaConstants.TicksPerMillisecond) : default(Duration?)
        );
        CreateMap<TimeSpan, Duration>().ConvertUsing(source => Duration.FromTimeSpan(source));
        CreateMap<TimeSpan?, Duration?>().ConvertUsing(source => source.HasValue ? Duration.FromTimeSpan(source.Value) : default(Duration?));
    }

    private void CreateMappingsForInstantConvertor()
    {
        CreateMap<DateTime, Instant>().ConvertUsing(
            source => Instant.FromDateTimeUtc(
                source.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(source, DateTimeKind.Utc) : source.ToUniversalTime()
            )
        );
        CreateMap<DateTime?, Instant?>().ConvertUsing(
            source => source.HasValue
                ? Instant.FromDateTimeUtc(
                    source.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(source.Value, DateTimeKind.Utc) : source.Value.ToUniversalTime()
                )
                : default
        );
        CreateMap<DateTimeOffset, Instant>().ConvertUsing(source => Instant.FromDateTimeOffset(source));
        CreateMap<DateTimeOffset?, Instant?>().ConvertUsing(source => source.HasValue ? Instant.FromDateTimeOffset(source.Value) : default(Instant?));
        CreateMap<Instant, DateTime>().ConvertUsing(source => source.ToDateTimeUtc());
        CreateMap<Instant, DateTimeOffset>().ConvertUsing(source => source.ToDateTimeOffset());
        CreateMap<Instant?, DateTime?>().ConvertUsing(source => source.HasValue ? source.Value.ToDateTimeUtc() : default(DateTime?));
        CreateMap<Instant?, DateTimeOffset?>().ConvertUsing(source => source.HasValue ? source.Value.ToDateTimeOffset() : default(DateTimeOffset?));
    }

    private void CreateMappingsForLocalDateConverter()
    {
        CreateMap<DateTime, LocalDate>().ConvertUsing(source => LocalDateTime.FromDateTime(source).Date);
        CreateMap<DateTime?, LocalDate?>().ConvertUsing(source => source.HasValue ? LocalDateTime.FromDateTime(source.Value).Date : default(LocalDate?));
        CreateMap<LocalDate, DateTime>().ConvertUsing(source => source.AtMidnight().ToDateTimeUnspecified());
        CreateMap<LocalDate?, DateTime?>().ConvertUsing(source => source.HasValue ? source.Value.AtMidnight().ToDateTimeUnspecified() : default(DateTime?));
    }

    private void CreateMappingsForLocalDateTimeConverter()
    {
        CreateMap<DateTime, LocalDateTime>().ConvertUsing(source => LocalDateTime.FromDateTime(source));
        CreateMap<DateTime?, LocalDateTime?>().ConvertUsing(source => source.HasValue ? LocalDateTime.FromDateTime(source.Value) : default(LocalDateTime?));
        CreateMap<LocalDateTime, DateTime>().ConvertUsing(source => source.ToDateTimeUnspecified());
        CreateMap<LocalDateTime?, DateTime?>().ConvertUsing(source => source.HasValue ? source.Value.ToDateTimeUnspecified() : default(DateTime?));
    }

    private void CreateMappingsForLocalTimeConverter()
    {
        CreateMap<DateTime, LocalTime>().ConvertUsing(source => LocalDateTime.FromDateTime(source).TimeOfDay);
        CreateMap<DateTime?, LocalTime?>().ConvertUsing(source => source.HasValue ? LocalDateTime.FromDateTime(source.Value).TimeOfDay : default(LocalTime?));
        CreateMap<LocalTime, DateTime>().ConvertUsing(source => source.On(new LocalDate(1, 1, 1)).ToDateTimeUnspecified());
        CreateMap<LocalTime, TimeSpan>().ConvertUsing(source => new TimeSpan(source.TickOfDay));
        CreateMap<LocalTime?, DateTime?>()
           .ConvertUsing(source => source.HasValue ? source.Value.On(new LocalDate(1, 1, 1)).ToDateTimeUnspecified() : default(DateTime?));
        CreateMap<LocalTime?, TimeSpan?>().ConvertUsing(source => source.HasValue ? new TimeSpan(source.Value.TickOfDay) : default(TimeSpan?));
        CreateMap<TimeSpan, LocalTime>().ConvertUsing(source => LocalTime.FromTicksSinceMidnight(source.Ticks));
        CreateMap<TimeSpan?, LocalTime?>().ConvertUsing(source => source.HasValue ? LocalTime.FromTicksSinceMidnight(source.Value.Ticks) : default(LocalTime?));
    }

    private void CreateMappingsForOffsetConverter()
    {
        CreateMap<Offset, TimeSpan>().ConvertUsing(source => source.ToTimeSpan());
        CreateMap<Offset?, TimeSpan?>().ConvertUsing(source => source.HasValue ? source.Value.ToTimeSpan() : default(TimeSpan?));
        CreateMap<TimeSpan, Offset>().ConvertUsing(source => Offset.FromTicks(source.Ticks));
        CreateMap<TimeSpan?, Offset?>().ConvertUsing(source => source.HasValue ? Offset.FromTicks(source.Value.Ticks) : default(Offset?));
    }

    private void CreateMappingsForOffsetDateTimeConverter()
    {
        CreateMap<DateTimeOffset, OffsetDateTime>().ConvertUsing(source => OffsetDateTime.FromDateTimeOffset(source));
        CreateMap<DateTimeOffset?, OffsetDateTime?>()
           .ConvertUsing(source => source.HasValue ? OffsetDateTime.FromDateTimeOffset(source.Value) : default(OffsetDateTime?));
        CreateMap<OffsetDateTime, DateTimeOffset>().ConvertUsing(source => source.ToDateTimeOffset());
        CreateMap<OffsetDateTime?, DateTimeOffset?>().ConvertUsing(source => source.HasValue ? source.Value.ToDateTimeOffset() : default(DateTimeOffset?));
    }

    private void CreateMappingsForPeriodConverter()
    {
        CreateMap<Period?, string?>().ConvertUsing(source => source == default ? default : source.ToString());
        CreateMap<string?, Period?>().ConvertUsing(source => ( source == default ? default : PeriodPattern.Roundtrip.Parse(source).Value ) ?? default(Period?));
    }
}
