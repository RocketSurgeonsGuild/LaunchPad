namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     A common interface used for definiting problems
/// </summary>
public interface IProblemDetailsData
{
    /// <summary>
    ///     Additional properties
    /// </summary>
    IDictionary<string, object?> Properties { get; }

    /// <summary>
    ///     Request title
    /// </summary>
    string? Title { get; }

    /// <summary>
    ///     Request Type
    /// </summary>
    string? Link { get; }

    /// <summary>
    ///     The instance for the request
    /// </summary>
    string? Instance { get; }
}
