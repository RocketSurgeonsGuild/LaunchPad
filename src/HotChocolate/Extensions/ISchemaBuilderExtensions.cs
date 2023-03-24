using HotChocolate;
using HotChocolate.Data;
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
           .AddConvention<IFilterConvention>(
                new FilterConventionExtension(
                    descriptor => descriptor
                                 .BindRuntimeType<DateTimeZone, ComparableOperationFilterInputType<DateTimeZone>>()
                                 .BindRuntimeType<Duration, ComparableOperationFilterInputType<Duration>>()
                                 .BindRuntimeType<Duration?, ComparableOperationFilterInputType<Duration>>()
                                 .BindRuntimeType<Instant, ComparableOperationFilterInputType<Instant>>()
                                 .BindRuntimeType<Instant?, ComparableOperationFilterInputType<Instant>>()
                                 .BindRuntimeType<IsoDayOfWeek, ComparableOperationFilterInputType<IsoDayOfWeek>>()
                                 .BindRuntimeType<IsoDayOfWeek?, ComparableOperationFilterInputType<IsoDayOfWeek>>()
                                 .BindRuntimeType<LocalDateTime, ComparableOperationFilterInputType<LocalDateTime>>()
                                 .BindRuntimeType<LocalDateTime?, ComparableOperationFilterInputType<LocalDateTime>>()
                                 .BindRuntimeType<LocalDate, ComparableOperationFilterInputType<LocalDate>>()
                                 .BindRuntimeType<LocalDate?, ComparableOperationFilterInputType<LocalDate>>()
                                 .BindRuntimeType<LocalTime, ComparableOperationFilterInputType<LocalTime>>()
                                 .BindRuntimeType<LocalTime?, ComparableOperationFilterInputType<LocalTime>>()
                                 .BindRuntimeType<OffsetDateTime, ComparableOperationFilterInputType<OffsetDateTime>>()
                                 .BindRuntimeType<OffsetDateTime?, ComparableOperationFilterInputType<OffsetDateTime>>()
                                 .BindRuntimeType<OffsetDate, ComparableOperationFilterInputType<OffsetDate>>()
                                 .BindRuntimeType<OffsetDate?, ComparableOperationFilterInputType<OffsetDate>>()
                                 .BindRuntimeType<OffsetTime, ComparableOperationFilterInputType<OffsetTime>>()
                                 .BindRuntimeType<OffsetTime?, ComparableOperationFilterInputType<OffsetTime>>()
                                 .BindRuntimeType<Offset, ComparableOperationFilterInputType<Offset>>()
                                 .BindRuntimeType<Offset?, ComparableOperationFilterInputType<Offset>>()
                                 .BindRuntimeType<Period, ComparableOperationFilterInputType<Period>>()
                                 .BindRuntimeType<ZonedDateTime, ComparableOperationFilterInputType<ZonedDateTime>>()
                                 .BindRuntimeType<ZonedDateTime?, ComparableOperationFilterInputType<ZonedDateTime>>()
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
}
