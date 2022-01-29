using HotChocolate;
using Rocket.Surgery.LaunchPad.HotChocolate.Types;

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
        return schemaBuilder
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
