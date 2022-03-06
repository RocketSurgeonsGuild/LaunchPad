namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

/// <summary>
///     Restful api method types
/// </summary>
internal enum RestfulApiMethod
{
    /// <summary>
    ///     This method returns a list or paged list
    /// </summary>
    List,

    /// <summary>
    ///     This method creates a new item
    /// </summary>
    Create,

    /// <summary>
    ///     This method returns a single item
    /// </summary>
    Read,

    /// <summary>
    ///     This method updates a single item
    /// </summary>
    Update,

    /// <summary>
    ///     This method removes an item
    /// </summary>
    Delete
}
