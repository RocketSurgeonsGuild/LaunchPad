using AutoMapper;
using NodaTime;
using Sample.Core.Domain;
using StronglyTypedIds;

namespace Sample.Core.Models;

/// <summary>
///     Unique Id of a launch record
/// </summary>
[StronglyTypedId(
    StronglyTypedIdBackingType.Guid,
    StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter | StronglyTypedIdConverter.TypeConverter
)]
public partial struct LaunchRecordId
{
}

/// <summary>
///     The launch record details
/// </summary>
public record LaunchRecordModel
{
    /// <summary>
    ///     The launch record id
    /// </summary>
    public LaunchRecordId Id { get; init; }

    /// <summary>
    ///     The launch partner
    /// </summary>
    public string Partner { get; init; } = null!;

    /// <summary>
    ///     The payload details
    /// </summary>
    public string Payload { get; init; } = null!;

    /// <summary>
    ///     The payload weight in Kg
    /// </summary>
    public long PayloadWeightKg { get; init; }

    /// <summary>
    ///     The actual launch date
    /// </summary>
    /// <remarks>
    ///     Will be updated when the launch happens
    /// </remarks>
    public Instant? ActualLaunchDate { get; init; }

    /// <summary>
    ///     The intended date for the launch
    /// </summary>
    public Instant ScheduledLaunchDate { get; init; }

    /// <summary>
    ///     The serial number of the reusable rocket
    /// </summary>
    public string RocketSerialNumber { get; init; } = null!;

    /// <summary>
    ///     The kind of rocket that will be launching
    /// </summary>
    public RocketType RocketType { get; init; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<LaunchRecord, LaunchRecordModel>();
        }
    }
}
