using HotChocolate.Types.NodaTime;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;

namespace Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints;

/// <summary>
///     Adds the NodaTime types to the HotChocolate schema
/// </summary>
/// <remarks>
///     The scalars are HotChocolate's own (<c>AddNodaTime()</c> from HotChocolate.Types.NodaTime): <c>DateTime</c> for
///     <see cref="OffsetDateTime" />, <c>Duration</c>, <c>LocalDate</c>, <c>LocalDateTime</c> and <c>LocalTime</c>. HotChocolate 16
///     has no scalar for <see cref="Instant" />, so the Joint binds it to the <c>DateTime</c> scalar as an
///     <see cref="OffsetDateTime" /> at UTC, which loses nothing. The Joint binds the same types to comparable filters. The other
///     NodaTime types (ZonedDateTime, OffsetDate, OffsetTime, Offset, Period, DateTimeZone, IsoDayOfWeek) have no scalar.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class NodaTimeHotChocolateJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) =>
        context
           .GetOrAdd(() => services.AddGraphQL())
           .AddNodaTime()
           .BindRuntimeType<Instant, DateTimeType>()
           .AddTypeConverter<Instant, OffsetDateTime>(instant => instant.WithOffset(Offset.Zero))
           .AddTypeConverter<OffsetDateTime, Instant>(dateTime => dateTime.ToInstant())
           .ConfigureSchema(sb => sb.AddNodaTimeFilters());
}
