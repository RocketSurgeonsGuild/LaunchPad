using Riok.Mapperly.Abstractions;
using Sample.Core.Domain;

namespace Sample.Core.Models;

/// <summary>
///     Unique Id of a rocket
/// </summary>
// The empty ID is read like any other, as the samples' clients send it in the bodies whose ID the route gives: the operations' validators
// reject it (IsAssigned).
[StrongId<Guid>(RejectEmpty = false)]
public readonly partial record struct RocketId;

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
