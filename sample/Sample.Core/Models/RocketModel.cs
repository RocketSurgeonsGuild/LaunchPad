using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Sample.Core.Domain;
using StronglyTypedIds;

namespace Sample.Core.Models;

/// <summary>
///     Unique Id of a rocket
/// </summary>
#pragma warning disable CA1036
[StronglyTypedId(Template.Guid, "guid-efcore")]
public partial struct RocketId { }
#pragma warning restore CA1036

/// <summary>
///     The details of a given rocket
/// </summary>
public record RocketModel
{
    /// <summary>
    ///     The unique rocket identifier
    /// </summary>
    public RocketId Id { get; init; }

    /// <summary>
    ///     The serial number of the rocket
    /// </summary>
    public string Sn { get; init; } = null!;

    /// <summary>
    ///     The type of the rocket
    /// </summary>
    public RocketType Type { get; init; }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    internal static partial class Mapper
    {
        [MapProperty(nameof(@ReadyRocket.SerialNumber), nameof(@RocketModel.Sn))]
        public static partial RocketModel Map(ReadyRocket launchRecord);
    }
}
