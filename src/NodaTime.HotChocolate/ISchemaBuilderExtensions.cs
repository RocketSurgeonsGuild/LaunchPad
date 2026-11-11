using HotChocolate;
using HotChocolate.Data.Filters;

using NodaTime;

namespace Rocket.Surgery.LaunchPad.NodaTime.HotChocolate;

/// <summary>
///     Schema builder extensions
/// </summary>
public static class ISchemaBuilderExtensions
{
    /// <summary>
    ///     Binds the NodaTime types HotChocolate has scalars for (<see cref="OffsetDateTime" />, <see cref="Duration" />,
    ///     <see cref="LocalDate" />, <see cref="LocalDateTime" /> and <see cref="LocalTime" />), and <see cref="Instant" />, which
    ///     the Joint binds to the <c>DateTime</c> scalar, to comparable filters.
    /// </summary>
    /// <remarks>
    ///     The scalars themselves come from HotChocolate's own <c>AddNodaTime()</c> on the request executor builder.
    /// </remarks>
    /// <param name="schemaBuilder"></param>
    /// <returns></returns>
    public static ISchemaBuilder AddNodaTimeFilters(this ISchemaBuilder schemaBuilder) =>
        schemaBuilder.AddConvention<IFilterConvention>(
            new FilterConventionExtension(
                descriptor => descriptor
                             .BindRuntimeType<Duration, ComparableOperationFilterInputType<Duration>>()
                             .BindRuntimeType<Duration?, ComparableOperationFilterInputType<Duration>>()
                             .BindRuntimeType<LocalDateTime, ComparableOperationFilterInputType<LocalDateTime>>()
                             .BindRuntimeType<LocalDateTime?, ComparableOperationFilterInputType<LocalDateTime>>()
                             .BindRuntimeType<LocalDate, ComparableOperationFilterInputType<LocalDate>>()
                             .BindRuntimeType<LocalDate?, ComparableOperationFilterInputType<LocalDate>>()
                             .BindRuntimeType<LocalTime, ComparableOperationFilterInputType<LocalTime>>()
                             .BindRuntimeType<LocalTime?, ComparableOperationFilterInputType<LocalTime>>()
                             .BindRuntimeType<OffsetDateTime, ComparableOperationFilterInputType<OffsetDateTime>>()
                             .BindRuntimeType<OffsetDateTime?, ComparableOperationFilterInputType<OffsetDateTime>>()
                             .BindRuntimeType<Instant, ComparableOperationFilterInputType<Instant>>()
                             .BindRuntimeType<Instant?, ComparableOperationFilterInputType<Instant>>()
                             // HotChocolate's AddNodaTime binds these to the NodaTime scalars, which take the names of the scalars the
                             // default filters of HotChocolate.Data are built on (DateTime, LocalDateTime, LocalDate, LocalTime); filters
                             // over the runtime types use the NodaTime scalars instead of adding a second scalar of the same name.
                             .BindRuntimeType<DateTimeOffset, ComparableOperationFilterInputType<DateTimeOffset>>()
                             .BindRuntimeType<DateTimeOffset?, ComparableOperationFilterInputType<DateTimeOffset>>()
                             .BindRuntimeType<DateTime, ComparableOperationFilterInputType<DateTime>>()
                             .BindRuntimeType<DateTime?, ComparableOperationFilterInputType<DateTime>>()
                             .BindRuntimeType<DateOnly, ComparableOperationFilterInputType<DateOnly>>()
                             .BindRuntimeType<DateOnly?, ComparableOperationFilterInputType<DateOnly>>()
                             .BindRuntimeType<TimeOnly, ComparableOperationFilterInputType<TimeOnly>>()
                             .BindRuntimeType<TimeOnly?, ComparableOperationFilterInputType<TimeOnly>>()
            )
        );
}
