using NodaTime;
using Riok.Mapperly.Abstractions;
using Sample.Core.Domain;
using StronglyTypedIds;

namespace Sample.Core.Models;

/// <summary>
///     Unique Id of a launch record
/// </summary>
#pragma warning disable CA1036
[StronglyTypedId(Template.Guid, "guid-efcore")]
public partial struct LaunchRecordId { }
#pragma warning restore CA1036

/// <summary>
///     The launch record details
/// </summary>
public partial record LaunchRecordModel
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
}

internal static partial class ModelMapper
{
    [MapperIgnoreSource(nameof(LaunchRecord.RocketId))]
    public static partial LaunchRecordModel Map(LaunchRecord launchRecord);

    public static partial IQueryable<LaunchRecordModel> ProjectTo(IQueryable<LaunchRecord> rocket);
}