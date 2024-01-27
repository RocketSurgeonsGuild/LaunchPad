using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using StrawberryShake.Serialization;

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
        services.AddSingleton<ISerializer, InstantSerializer>();
        services.AddSingleton<ISerializer, LocalDateSerializer>();
        services.AddSingleton<ISerializer, LocalTimeSerializer>();
        services.AddSingleton<ISerializer, LocalDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetTimeSerializer>();
        services.AddSingleton<ISerializer, PeriodSerializer>();
        services.AddSingleton<ISerializer, DurationSerializer>();
        services.AddSingleton<ISerializer, ZonedDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetSerializer>();
        services.AddSingleton<ISerializer, IsoDayOfWeekSerializer>();
    }
}
