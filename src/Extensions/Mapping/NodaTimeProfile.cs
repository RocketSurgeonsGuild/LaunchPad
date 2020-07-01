using System;
using AutoMapper;
using NodaTime;
using Rocket.Surgery.Extensions.AutoMapper.Converters;

namespace Rocket.Surgery.Extensions.AutoMapper
{
    /// <summary>
    /// NodaTimeProfile.
    /// Implements the <see cref="Profile" />
    /// </summary>
    /// <seealso cref="Profile" />
    public class NodaTimeProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodaTimeProfile" /> class.
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
        /// Gets the name of the profile.
        /// </summary>
        /// <value>The name of the profile.</value>
        public override string ProfileName => nameof(NodaTimeProfile);

        private void CreateMappingsForDurationConverter()
        {
            var converter = new DurationConverter();
            CreateMap<Duration, TimeSpan>().ConvertUsing(converter);
            CreateMap<Duration?, TimeSpan?>().ConvertUsing(converter);
            CreateMap<TimeSpan, Duration>().ConvertUsing(converter);
            CreateMap<TimeSpan?, Duration?>().ConvertUsing(converter);
            CreateMap<Duration, long>().ConvertUsing(converter);
            CreateMap<Duration?, long?>().ConvertUsing(converter);
            CreateMap<long, Duration>().ConvertUsing(converter);
            CreateMap<long?, Duration?>().ConvertUsing(converter);
            CreateMap<Duration, int>().ConvertUsing(converter);
            CreateMap<Duration?, int?>().ConvertUsing(converter);
            CreateMap<int, Duration>().ConvertUsing(converter);
            CreateMap<int?, Duration?>().ConvertUsing(converter);
            CreateMap<Duration, double>().ConvertUsing(converter);
            CreateMap<Duration?, double?>().ConvertUsing(converter);
            CreateMap<double, Duration>().ConvertUsing(converter);
            CreateMap<double?, Duration?>().ConvertUsing(converter);
            CreateMap<Duration, decimal>().ConvertUsing(converter);
            CreateMap<Duration?, decimal?>().ConvertUsing(converter);
            CreateMap<decimal, Duration>().ConvertUsing(converter);
            CreateMap<decimal?, Duration?>().ConvertUsing(converter);
        }

        private void CreateMappingsForInstantConvertor()
        {
            var converter = new InstantConverter();
            CreateMap<Instant, DateTime>().ConvertUsing(converter);
            CreateMap<Instant?, DateTime?>().ConvertUsing(converter);
            CreateMap<Instant, DateTimeOffset>().ConvertUsing(converter);
            CreateMap<Instant?, DateTimeOffset?>().ConvertUsing(converter);
            CreateMap<DateTime, Instant>().ConvertUsing(converter);
            CreateMap<DateTime?, Instant?>().ConvertUsing(converter);
            CreateMap<DateTimeOffset, Instant>().ConvertUsing(converter);
            CreateMap<DateTimeOffset?, Instant?>().ConvertUsing(converter);
        }

        private void CreateMappingsForLocalDateConverter()
        {
            var converter = new LocalDateConverter();
            CreateMap<LocalDate, DateTime>().ConvertUsing(converter);
            CreateMap<LocalDate?, DateTime?>().ConvertUsing(converter);
            CreateMap<DateTime, LocalDate>().ConvertUsing(converter);
            CreateMap<DateTime?, LocalDate?>().ConvertUsing(converter);
        }

        private void CreateMappingsForLocalDateTimeConverter()
        {
            var converter = new LocalDateTimeConverter();
            CreateMap<LocalDateTime, DateTime>().ConvertUsing(converter);
            CreateMap<LocalDateTime?, DateTime?>().ConvertUsing(converter);
            CreateMap<DateTime, LocalDateTime>().ConvertUsing(converter);
            CreateMap<DateTime?, LocalDateTime?>().ConvertUsing(converter);
        }

        private void CreateMappingsForLocalTimeConverter()
        {
            var converter = new LocalTimeConverter();
            CreateMap<LocalTime, TimeSpan>().ConvertUsing(converter);
            CreateMap<LocalTime?, TimeSpan?>().ConvertUsing(converter);
            CreateMap<TimeSpan, LocalTime>().ConvertUsing(converter);
            CreateMap<TimeSpan?, LocalTime?>().ConvertUsing(converter);
            CreateMap<LocalTime, DateTime>().ConvertUsing(converter);
            CreateMap<LocalTime?, DateTime?>().ConvertUsing(converter);
            CreateMap<DateTime, LocalTime>().ConvertUsing(converter);
            CreateMap<DateTime?, LocalTime?>().ConvertUsing(converter);
        }

        private void CreateMappingsForOffsetConverter()
        {
            var converter = new OffsetConverter();
            CreateMap<Offset, TimeSpan>().ConvertUsing(converter);
            CreateMap<Offset?, TimeSpan?>().ConvertUsing(converter);
            CreateMap<TimeSpan, Offset>().ConvertUsing(converter);
            CreateMap<TimeSpan?, Offset?>().ConvertUsing(converter);
        }

        private void CreateMappingsForOffsetDateTimeConverter()
        {
            var converter = new OffsetDateTimeConverter();
            CreateMap<OffsetDateTime, DateTimeOffset>().ConvertUsing(converter);
            CreateMap<OffsetDateTime?, DateTimeOffset?>().ConvertUsing(converter);
            CreateMap<DateTimeOffset, OffsetDateTime>().ConvertUsing(converter);
            CreateMap<DateTimeOffset?, OffsetDateTime?>().ConvertUsing(converter);
        }

        private void CreateMappingsForPeriodConverter()
        {
            var converter = new PeriodConverter();
            CreateMap<Period, string>().ConvertUsing(converter);
            CreateMap<string, Period>().ConvertUsing(converter);
        }
    }
}