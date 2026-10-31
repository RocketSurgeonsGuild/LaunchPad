using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Joints;

/// <summary>
///     Adds the NodaTime serializers to StrawberryShake
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class StrawberryShakeJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
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
