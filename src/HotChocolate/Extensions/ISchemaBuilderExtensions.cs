using HotChocolate;
using HotChocolate.Data.Filters;
using HotChocolate.Types.NodaTime;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Extensions;

/// <summary>
///     Schema builder extensions
/// </summary>
public static class ISchemaBuilderExtensions
{
    /// <summary>
    ///     Add nodatime types to HotChocolate
    /// </summary>
    /// <param name="schemaBuilder"></param>
    /// <returns></returns>
    public static ISchemaBuilder AddNodaTime(this ISchemaBuilder schemaBuilder)
    {
        schemaBuilder
           .AddFiltering()
           .AddConvention<IFilterConvention>(
                new FilterConventionExtension(
                    descriptor => descriptor
                                 .BindRuntimeType<Duration, ComparableOperationFilterInputType<Duration>>()
                                 .BindRuntimeType<DateTimeZone, ComparableOperationFilterInputType<DateTimeZone>>()
                                 .BindRuntimeType<Duration, ComparableOperationFilterInputType<Duration>>()
                                 .BindRuntimeType<Instant, ComparableOperationFilterInputType<Instant>>()
                                 .BindRuntimeType<IsoDayOfWeek, ComparableOperationFilterInputType<IsoDayOfWeek>>()
                                 .BindRuntimeType<LocalDateTime, ComparableOperationFilterInputType<LocalDateTime>>()
                                 .BindRuntimeType<LocalDate, ComparableOperationFilterInputType<LocalDate>>()
                                 .BindRuntimeType<LocalTime, ComparableOperationFilterInputType<LocalTime>>()
                                 .BindRuntimeType<OffsetDateTime, ComparableOperationFilterInputType<OffsetDateTime>>()
                                 .BindRuntimeType<OffsetDate, ComparableOperationFilterInputType<OffsetDate>>()
                                 .BindRuntimeType<OffsetTime, ComparableOperationFilterInputType<OffsetTime>>()
                                 .BindRuntimeType<Offset, ComparableOperationFilterInputType<Offset>>()
                                 .BindRuntimeType<Period, ComparableOperationFilterInputType<Period>>()
                                 .BindRuntimeType<ZonedDateTime, ComparableOperationFilterInputType<ZonedDateTime>>()
                )
            );

        return schemaBuilder
              .AddType<DateTimeZoneType>()
              .AddType(new DurationType(DurationPattern.JsonRoundtrip, DurationPattern.Roundtrip))
              .AddType(new InstantType(InstantPattern.General, InstantPattern.ExtendedIso, new InstantDateTimeOffsetPattern()))
              .AddType<IsoDayOfWeekType>()
              .AddType(new LocalDateTimeType(LocalDateTimePattern.GeneralIso, LocalDateTimePattern.ExtendedIso, LocalDateTimePattern.BclRoundtrip))
              .AddType(new LocalDateType(LocalDatePattern.Iso, LocalDatePattern.FullRoundtrip))
              .AddType(new LocalTimeType(LocalTimePattern.ExtendedIso, LocalTimePattern.GeneralIso))
              .AddType(new OffsetDateTimeType(OffsetDateTimePattern.GeneralIso, OffsetDateTimePattern.FullRoundtrip))
              .AddType(new OffsetDateType(OffsetDatePattern.GeneralIso, OffsetDatePattern.FullRoundtrip))
              .AddType(new OffsetTimeType(OffsetTimePattern.Rfc3339, OffsetTimePattern.GeneralIso, OffsetTimePattern.ExtendedIso))
              .AddType(new OffsetType(OffsetPattern.GeneralInvariant, OffsetPattern.GeneralInvariantWithZ))
              .AddType(new PeriodType(PeriodPattern.Roundtrip, PeriodPattern.NormalizingIso))
              .AddType<ZonedDateTimeType>();
    }

    /// <summary>
    ///     Represents a fixed (and calendar-independent) length of time.
    /// </summary>
    public class DurationType : StringToStructBaseType<Duration>
    {
        private readonly IPattern<Duration>[] _allowedPatterns;
        private readonly IPattern<Duration> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="DurationType" />.
        /// </summary>
        public DurationType() : this(DurationPattern.Roundtrip)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="DurationType" />.
        /// </summary>
        public DurationType(params IPattern<Duration>[] allowedPatterns) : base("Duration")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description = "Represents a fixed (and calendar-independent) length of time.";
        }

        /// <inheritdoc />
        protected override string Serialize(Duration runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out Duration? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     Represents an instant on the global timeline, with nanosecond resolution.
    /// </summary>
    public class InstantType : StringToStructBaseType<Instant>
    {
        private readonly IPattern<Instant>[] _allowedPatterns;
        private readonly IPattern<Instant> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="InstantType" />.
        /// </summary>
        public InstantType() : this(InstantPattern.ExtendedIso)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="InstantType" />.
        /// </summary>
        public InstantType(params IPattern<Instant>[] allowedPatterns) : base("Instant")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description = "Represents an instant on the global timeline, with nanosecond resolution.";
        }

        /// <inheritdoc />
        protected override string Serialize(Instant runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out Instant? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }


    /// <summary>
    ///     A date and time in a particular calendar system.
    /// </summary>
    public class LocalDateTimeType : StringToStructBaseType<LocalDateTime>
    {
        private readonly IPattern<LocalDateTime>[] _allowedPatterns;
        private readonly IPattern<LocalDateTime> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="LocalDateTimeType" />.
        /// </summary>
        public LocalDateTimeType() : this(LocalDateTimePattern.ExtendedIso)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="LocalDateTimeType" />.
        /// </summary>
        public LocalDateTimeType(params IPattern<LocalDateTime>[] allowedPatterns) : base("LocalDateTime")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description = "A date and time in a particular calendar system.";
        }

        /// <inheritdoc />
        protected override string Serialize(LocalDateTime runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out LocalDateTime? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     LocalDate is an immutable struct representing a date within the calendar,
    ///     with no reference to a particular time zone or time of day.
    /// </summary>
    public class LocalDateType : StringToStructBaseType<LocalDate>
    {
        private readonly IPattern<LocalDate>[] _allowedPatterns;
        private readonly IPattern<LocalDate> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="LocalDateType" />.
        /// </summary>
        public LocalDateType() : this(LocalDatePattern.Iso)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="LocalDateType" />.
        /// </summary>
        public LocalDateType(params IPattern<LocalDate>[] allowedPatterns) : base("LocalDate")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description =
                "LocalDate is an immutable struct representing a date within the calendar, with no reference to a particular time zone or time of day.";
        }

        /// <inheritdoc />
        protected override string Serialize(LocalDate runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out LocalDate? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     LocalTime is an immutable struct representing a time of day,
    ///     with no reference to a particular calendar, time zone or date.
    /// </summary>
    public class LocalTimeType : StringToStructBaseType<LocalTime>
    {
        private readonly IPattern<LocalTime>[] _allowedPatterns;
        private readonly IPattern<LocalTime> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="LocalTimeType" />.
        /// </summary>
        public LocalTimeType() : this(LocalTimePattern.ExtendedIso)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="LocalTimeType" />.
        /// </summary>
        public LocalTimeType(params IPattern<LocalTime>[] allowedPatterns) : base("LocalTime")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description = "LocalTime is an immutable struct representing a time of day, with no reference to a particular calendar, time zone or date.";
        }

        /// <inheritdoc />
        protected override string Serialize(LocalTime runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out LocalTime? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     A local date and time in a particular calendar system, combined with an offset from UTC.
    /// </summary>
    public class OffsetDateTimeType : StringToStructBaseType<OffsetDateTime>
    {
        private readonly IPattern<OffsetDateTime>[] _allowedPatterns;
        private readonly IPattern<OffsetDateTime> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetDateTimeType" />.
        /// </summary>
        public OffsetDateTimeType() : this(OffsetDateTimePattern.ExtendedIso)
        {
            // Backwards compatibility with the original code's behavior
            _serializationPattern = OffsetDateTimePattern.GeneralIso;
            _allowedPatterns = new IPattern<OffsetDateTime>[] { OffsetDateTimePattern.ExtendedIso };
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetDateTimeType" />.
        /// </summary>
        public OffsetDateTimeType(params IPattern<OffsetDateTime>[] allowedPatterns) : base("OffsetDateTime")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = _allowedPatterns[0];
            Description = "A local date and time in a particular calendar system, combined with an offset from UTC.";
        }

        /// <inheritdoc />
        protected override string Serialize(OffsetDateTime runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out OffsetDateTime? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     A combination of a LocalDate and an Offset,
    ///     to represent a date at a specific offset from UTC but
    ///     without any time-of-day information.
    /// </summary>
    public class OffsetDateType : StringToStructBaseType<OffsetDate>
    {
        private readonly IPattern<OffsetDate>[] _allowedPatterns;
        private readonly IPattern<OffsetDate> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetDateType" />.
        /// </summary>
        public OffsetDateType() : this(OffsetDatePattern.GeneralIso)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetDateType" />.
        /// </summary>
        public OffsetDateType(params IPattern<OffsetDate>[] allowedPatterns) : base("OffsetDate")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description =
                "A combination of a LocalDate and an Offset, to represent a date at a specific offset from UTC but without any time-of-day information.";
        }

        /// <inheritdoc />
        protected override string Serialize(OffsetDate runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out OffsetDate? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     A combination of a LocalTime and an Offset, to represent a time-of-day at a specific offset
    ///     from UTC but without any date information.
    /// </summary>
    public class OffsetTimeType : StringToStructBaseType<OffsetTime>
    {
        private readonly IPattern<OffsetTime>[] _allowedPatterns;
        private readonly IPattern<OffsetTime> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetTimeType" />.
        /// </summary>
        public OffsetTimeType() : this(OffsetTimePattern.GeneralIso)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetTimeType" />.
        /// </summary>
        public OffsetTimeType(params IPattern<OffsetTime>[] allowedPatterns) : base("OffsetTime")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = _allowedPatterns[0];

            Description =
                "A combination of a LocalTime and an Offset, to represent a time-of-day at a specific offset from UTC but without any date information.";
        }

        /// <inheritdoc />
        protected override string Serialize(OffsetTime runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out OffsetTime? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     An offset from UTC in seconds.
    ///     A positive value means that the local time is ahead of UTC (e.g. for Europe);
    ///     a negative value means that the local time is behind UTC (e.g. for America).
    /// </summary>
    public class OffsetType : StringToStructBaseType<Offset>
    {
        private readonly IPattern<Offset>[] _allowedPatterns;
        private readonly IPattern<Offset> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetType" />.
        /// </summary>
        public OffsetType() : this(OffsetPattern.GeneralInvariantWithZ)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="OffsetType" />.
        /// </summary>
        public OffsetType(params IPattern<Offset>[] allowedPatterns) : base("Offset")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description =
                "An offset from UTC in seconds. A positive value means that the local time is ahead of UTC (e.g. for Europe); a negative value means that the local time is behind UTC (e.g. for America).";
        }

        /// <inheritdoc />
        protected override string Serialize(Offset runtimeValue)
        {
            return _serializationPattern
               .Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out Offset? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }

    /// <summary>
    ///     Represents a period of time expressed in human chronological terms:
    ///     hours, days, weeks, months and so on.
    /// </summary>
    public class PeriodType : StringToClassBaseType<Period>
    {
        private readonly IPattern<Period>[] _allowedPatterns;
        private readonly IPattern<Period> _serializationPattern;

        /// <summary>
        ///     Initializes a new instance of <see cref="PeriodType" />.
        /// </summary>
        public PeriodType() : this(PeriodPattern.Roundtrip)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="PeriodType" />.
        /// </summary>
        public PeriodType(params IPattern<Period>[] allowedPatterns) : base("Period")
        {
            _allowedPatterns = allowedPatterns;
            _serializationPattern = allowedPatterns[0];
            Description = "Represents a period of time expressed in human chronological terms: hours, days, weeks, months and so on.";
        }

        /// <inheritdoc />
        protected override string Serialize(Period runtimeValue)
        {
            return _serializationPattern.Format(runtimeValue);
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(
            string resultValue,
            [NotNullWhen(true)] out Period? runtimeValue
        )
        {
            return _allowedPatterns.TryParse(resultValue, out runtimeValue);
        }
    }
}

internal static class PatternExtensions
{
    public static bool TryParse<NodaTimeType>(
        this IPattern<NodaTimeType> pattern,
        string text,
        [NotNullWhen(true)] out NodaTimeType? output
    )
        where NodaTimeType : struct
    {
        var result = pattern.Parse(text);

        if (result.Success)
        {
            output = result.Value;
            return true;
        }

        output = null;
        return false;
    }

    public static bool TryParse<NodaTimeType>(
        this IPattern<NodaTimeType> pattern,
        string text,
        [NotNullWhen(true)] out NodaTimeType? output
    )
        where NodaTimeType : class
    {
        var result = pattern.Parse(text);

        if (result.Success)
        {
            output = result.Value;
            return true;
        }

        output = null;
        return false;
    }

    public static bool TryParse<NodaTimeType>(
        this IPattern<NodaTimeType>[] patterns,
        string text,
        [NotNullWhen(true)] out NodaTimeType? output
    )
        where NodaTimeType : struct
    {
        foreach (var pattern in patterns)
        {
            if (pattern.TryParse(text, out output))
            {
                return true;
            }
        }

        output = default;
        return false;
    }

    public static bool TryParse<NodaTimeType>(
        this IPattern<NodaTimeType>[] patterns,
        string text,
        [NotNullWhen(true)] out NodaTimeType? output
    )
        where NodaTimeType : class
    {
        foreach (var pattern in patterns)
        {
            if (pattern.TryParse(text, out output))
            {
                return true;
            }
        }

        output = default;
        return false;
    }
}
