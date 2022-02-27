using AutoMapper;
using Sample.Core.Domain;
using StronglyTypedIds;

namespace Sample.Core.Models;

/// <summary>
///     Unique Id of a rocket
/// </summary>
[StronglyTypedId(
    StronglyTypedIdBackingType.Guid,
    StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter | StronglyTypedIdConverter.TypeConverter
)]
public partial struct RocketId
{
}

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

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ReadyRocket, RocketModel>()
               .ForMember(z => z.Sn, x => x.MapFrom(z => z.SerialNumber));
        }
    }
}
