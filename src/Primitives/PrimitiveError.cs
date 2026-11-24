namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Why a domain primitive rejected a value: a code (<c>LENGTH</c>, <c>FORMAT</c>, <c>RANGE</c>, ...) and a message.
/// </summary>
/// <param name="Code">The code, or <see langword="null" /> when there is no error.</param>
/// <param name="Message">The message.</param>
[PublicAPI]
public readonly record struct PrimitiveError(string? Code, string? Message)
{
    /// <summary>
    ///     No error.
    /// </summary>
    public static PrimitiveError None => default;

    /// <summary>
    ///     Whether this is an error.
    /// </summary>
    public bool IsError => Code is not null;

    /// <summary>
    ///     Creates an error.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <returns>The error.</returns>
    public static PrimitiveError Create(string code, string message) => new(code, message);
}
