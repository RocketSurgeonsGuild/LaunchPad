﻿using Riok.Mapperly.Abstractions;
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
}

internal static partial class ModelMapper
{
    [MapProperty(nameof(ReadyRocket.SerialNumber), nameof(RocketModel.Sn))]
    public static partial RocketModel Map(ReadyRocket rocket);

    public static partial IQueryable<RocketModel> ProjectTo(IQueryable<ReadyRocket> rocket);
}