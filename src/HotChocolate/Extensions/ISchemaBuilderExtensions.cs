using HotChocolate;
using HotChocolate.Data.Filters;
using HotChocolate.Types.NodaTime;
using NodaTime;

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
              .AddType<DurationType>()
              .AddType<DateTimeZoneType>()
              .AddType<DurationType>()
              .AddType<InstantType>()
              .AddType<IsoDayOfWeekType>()
              .AddType<LocalDateTimeType>()
              .AddType<LocalDateType>()
              .AddType<LocalTimeType>()
              .AddType<OffsetDateTimeType>()
              .AddType<OffsetDateType>()
              .AddType<OffsetTimeType>()
              .AddType<OffsetType>()
              .AddType<PeriodType>()
              .AddType<ZonedDateTimeType>();
    }
}
