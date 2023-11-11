using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Conventions;

/// <summary>
///     Adds support for spatial types into STJ
/// </summary>
[PublicAPI]
[ExportConvention]
public class StrawberryShakeConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSerializer<InstantSerializer>();
        services.AddSerializer<LocalDateSerializer>();
        services.AddSerializer<LocalTimeSerializer>();
        services.AddSerializer<LocalDateTimeSerializer>();
        services.AddSerializer<OffsetDateTimeSerializer>();
        services.AddSerializer<OffsetTimeSerializer>();
        services.AddSerializer<PeriodSerializer>();
        services.AddSerializer<DurationSerializer>();
        services.AddSerializer<ZonedDateTimeSerializer>();
        services.AddSerializer<OffsetSerializer>();
        services.AddSerializer<IsoDayOfWeekSerializer>();
    }
}
