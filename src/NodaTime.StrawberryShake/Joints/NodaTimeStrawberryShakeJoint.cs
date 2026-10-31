using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints;

/// <summary>
///     Adds the NodaTime serializers to StrawberryShake
/// </summary>
/// <remarks>
///     <para>
///         Every NodaTime scalar that <c>AddNodaTime</c> puts in a HotChocolate schema has a serializer here, and all of them are registered.
///         <see cref="OffsetDateSerializer" /> and <see cref="DateTimeZoneSerializer" /> existed but were not registered before this package
///         (RocketSurgeonsGuild/LaunchPad#3066). They are registered now because a generated client resolves a serializer for every scalar its
///         operations use, and fails to create its serializer resolver for all operations when one is missing.
///     </para>
///     <para>
///         <see cref="ZonedDateTimeSerializer" /> and <see cref="DateTimeZoneSerializer" /> resolve the <see cref="IDateTimeZoneProvider" /> that the
///         <c>TimeJoint</c> of Rocket.Surgery.LaunchPad.NodaTime registers.
///     </para>
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NodaTimeStrawberryShakeJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddSingleton<ISerializer, InstantSerializer>();
        services.AddSingleton<ISerializer, LocalDateSerializer>();
        services.AddSingleton<ISerializer, LocalTimeSerializer>();
        services.AddSingleton<ISerializer, LocalDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetDateSerializer>();
        services.AddSingleton<ISerializer, OffsetTimeSerializer>();
        services.AddSingleton<ISerializer, PeriodSerializer>();
        services.AddSingleton<ISerializer, DurationSerializer>();
        services.AddSingleton<ISerializer, ZonedDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetSerializer>();
        services.AddSingleton<ISerializer, IsoDayOfWeekSerializer>();
        services.AddSingleton<ISerializer, DateTimeZoneSerializer>();
    }
}
