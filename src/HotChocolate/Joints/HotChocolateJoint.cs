using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Joints;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     Hot Chocolate Joint
/// </summary>
[PublicAPI]
[DovetailExport]
[AfterJoint<TimeJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class HotChocolateJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        if (services.FirstOrDefault(z => z.ServiceType == typeof(IDateTimeZoneProvider) && z.ImplementationInstance is { }) is not
            { ImplementationInstance: IDateTimeZoneProvider dateTimeZoneProvider })
        {
            dateTimeZoneProvider = new DateTimeZoneCache(context.Get<PrimitiveOptions>()?.DateTimeZoneSource ?? TzdbDateTimeZoneSource.Default);
        }

        services
           .ConfigureOptions<HotChocolateContextDataConfigureOptions>();
        context
           .GetOrAdd(() => services.AddGraphQL())
           .ConfigureSchema(sb => sb.AddNodaTime(dateTimeZoneProvider))
           .AddInstrumentation();
    }
}
