using System.Text.Json;
using NodaTime;
using NodaTime.Text;
using Serilog.Core;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Foundation;

internal class NodaTimeDestructuringPolicy : IDestructuringPolicy
{
    private readonly ZonedDateTimePattern _zonedDateTimePattern;

    public NodaTimeDestructuringPolicy(IDateTimeZoneProvider provider)
    {
        _zonedDateTimePattern = ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider);
    }

    public bool TryDestructure(object value, ILogEventPropertyValueFactory _, out LogEventPropertyValue? result)
    {
        ScalarValue a;
        if (value is Instant instant1)
        {
            result = new ScalarValue(InstantPattern.ExtendedIso.Format(instant1));
            return true;
        }

        if (value is LocalDateTime localDateTime)
        {
            result = new ScalarValue(LocalDateTimePattern.ExtendedIso.Format(localDateTime));
            return true;
        }

        if (value is LocalDate localDate)
        {
            result = new ScalarValue(LocalDatePattern.Iso.Format(localDate));
            return true;
        }

        if (value is LocalTime localTime)
        {
            result = new ScalarValue(LocalTimePattern.ExtendedIso.Format(localTime));
            return true;
        }

        if (value is OffsetDateTime offsetDateTime)
        {
            result = new ScalarValue(OffsetDateTimePattern.ExtendedIso.Format(offsetDateTime));
            return true;
        }

        if (value is OffsetDate offsetDate)
        {
            result = new ScalarValue(OffsetDatePattern.GeneralIso.Format(offsetDate));
            return true;
        }

        if (value is OffsetTime offsetTime)
        {
            result = new ScalarValue(OffsetTimePattern.ExtendedIso.Format(offsetTime));
            return true;
        }

        if (value is ZonedDateTime zonedDateTime)
        {
            result = new ScalarValue(_zonedDateTimePattern.Format(zonedDateTime));
            return true;
        }

        if (value is DateTimeZone dateTimeZone)
        {
            result = new ScalarValue(dateTimeZone.Id);
            return true;
        }

        if (value is Duration duration)
        {
            result = new ScalarValue(DurationPattern.Roundtrip.Format(duration));
            return true;
        }

        if (value is Period period)
        {
            result = new ScalarValue(PeriodPattern.NormalizingIso.Format(period));
            return true;
        }

        if (value is Interval interval)
        {
            var values = new List<LogEventProperty>();
            if (interval.HasStart)
            {
                values.Add(new LogEventProperty("Start", _.CreatePropertyValue(interval.Start)));
            }

            if (interval.HasEnd)
            {
                values.Add(new LogEventProperty("End", _.CreatePropertyValue(interval.End)));
            }

            result = new StructureValue(values);
            return true;
        }

        result = null;
        return false;
    }
}
