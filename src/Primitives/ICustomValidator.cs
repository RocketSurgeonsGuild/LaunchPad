namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     A rule of a domain primitive written as code, for <c>[CustomValidator&lt;TValidator&gt;]</c>. It runs after the domain primitive's
///     other rules.
/// </summary>
/// <typeparam name="T">The value.</typeparam>
[PublicAPI]
public interface ICustomValidator<in T>
{
    /// <summary>
    ///     Validates the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><see cref="PrimitiveError.None" /> when the value is accepted, otherwise why it is not.</returns>
    abstract static PrimitiveError Validate(T value);
}
