using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.NodaTime.Joints;

namespace Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.Joints;

/// <summary>
///     Adds the NodaTime types to the HotChocolate schema
/// </summary>
/// <remarks>
///     The ZonedDateTime type parses with the <see cref="IDateTimeZoneProvider" /> instance registered in the services (the one the
///     <see cref="TimeJoint" /> registers, so this Joint runs after it), or with a cache over <see cref="NodaTimeOptions.DateTimeZoneSource" />
///     when there is none.
/// </remarks>
[PublicAPI]
[DovetailExport]
[AfterJoint<TimeJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class NodaTimeHotChocolateJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        if (services.FirstOrDefault(z => z.ServiceType == typeof(IDateTimeZoneProvider) && z.ImplementationInstance is { }) is not
            { ImplementationInstance: IDateTimeZoneProvider dateTimeZoneProvider })
        {
            dateTimeZoneProvider = new DateTimeZoneCache(context.Get<NodaTimeOptions>()?.DateTimeZoneSource ?? TzdbDateTimeZoneSource.Default);
        }

        context
           .GetOrAdd(() => services.AddGraphQL())
           .ConfigureSchema(sb => sb.AddNodaTime(dateTimeZoneProvider));
    }
}
