namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

/// <summary>
///     The behavior for matching the type of a convention parameter.
/// </summary>
internal enum ApiConventionTypeMatchBehavior
{
    /// <summary>
    ///     Matches any type. Use this if the parameter does not need to be matched.
    /// </summary>
    Any,

    /// <summary>
    ///     The parameter in the convention is the exact type or a subclass of the type
    ///     specified in the convention.
    /// </summary>
    AssignableFrom,
}
