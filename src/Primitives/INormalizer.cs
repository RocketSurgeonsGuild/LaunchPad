namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Normalizes a value before a domain primitive validates it, for <c>[Normalize&lt;TNormalizer&gt;]</c>.
/// </summary>
/// <typeparam name="T">The value.</typeparam>
[PublicAPI]
public interface INormalizer<T>
{
    /// <summary>
    ///     Normalizes the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The normalized value.</returns>
    abstract static T Normalize(T value);
}
