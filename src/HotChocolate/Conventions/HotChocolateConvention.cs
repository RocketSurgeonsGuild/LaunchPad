using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.TimeZones;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using IConventionContext = Rocket.Surgery.Conventions.IConventionContext;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Conventions;

/// <summary>
///     Hot Chocolate convention
/// </summary>
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(NodaTimeConvention))]
[ConventionCategory(ConventionCategory.Application)]
public class HotChocolateConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        if (services.FirstOrDefault(z => z.ServiceType == typeof(IDateTimeZoneProvider) && z.ImplementationInstance != null) is not
            { ImplementationInstance: IDateTimeZoneProvider dateTimeZoneProvider, })
            dateTimeZoneProvider = new DateTimeZoneCache(context.Get<FoundationOptions>()?.DateTimeZoneSource ?? TzdbDateTimeZoneSource.Default);

        services
           .ConfigureOptions<HotChocolateContextDataConfigureOptions>();
        context
           .GetOrAdd(() => services.AddGraphQL())
           .ConfigureSchema(sb => sb.AddNodaTime(dateTimeZoneProvider))
           .AddInstrumentation();
    }
}
