using HotChocolate.Types.NodaTime;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints;

/// <summary>
///     Adds the NodaTime types to the HotChocolate schema
/// </summary>
/// <remarks>
///     The scalars are HotChocolate's own (<c>AddNodaTime()</c> from HotChocolate.Types.NodaTime): <c>DateTime</c> for
///     <see cref="global::NodaTime.OffsetDateTime" />, <c>Duration</c>, <c>LocalDate</c>, <c>LocalDateTime</c> and <c>LocalTime</c>. The
///     Joint binds the same types to comparable filters. HotChocolate 16 has no scalar for the other NodaTime types (Instant,
///     ZonedDateTime, OffsetDate, OffsetTime, Offset, Period, DateTimeZone, IsoDayOfWeek).
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
           .ConfigureSchema(sb => sb.AddNodaTimeFilters());
}
