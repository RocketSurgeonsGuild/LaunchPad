namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Defines the set of data required to apply authorization rules to a resource.
/// </summary>
public interface IAuthorizeData
{
    /// <summary>
    ///     Gets or sets the policy name that determines access to the resource.
    /// </summary>
    string? Policy { get; set; }
}
