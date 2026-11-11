using Microsoft.Extensions.DependencyInjection;

using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints;

/// <summary>
///     Adds the NodaTime serializers to StrawberryShake
/// </summary>
/// <remarks>
///     Every NodaTime scalar that HotChocolate 16's <c>AddNodaTime</c> puts in a schema has a serializer here, and all of them are registered:
///     <c>DateTime</c> (<see cref="global::NodaTime.OffsetDateTime" />), <c>Duration</c>, <c>LocalDate</c>, <c>LocalDateTime</c> and
///     <c>LocalTime</c>. A generated client resolves a serializer for every scalar its operations use, and cannot create its serializer
///     resolver, for any operation, when one of them is missing. A client maps the scalars to the NodaTime types with <c>@runtimeType</c> in its
///     <c>schema.extensions.graphql</c>.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NodaTimeStrawberryShakeJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddSingleton<ISerializer, LocalDateSerializer>();
        services.AddSingleton<ISerializer, LocalTimeSerializer>();
        services.AddSingleton<ISerializer, LocalDateTimeSerializer>();
        services.AddSingleton<ISerializer, OffsetDateTimeSerializer>();
        services.AddSingleton<ISerializer, DurationSerializer>();
    }
}
