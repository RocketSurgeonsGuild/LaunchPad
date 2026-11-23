namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Thrown when a domain primitive is created from a value its rules reject, unless <c>LaunchPadPrimitivesExceptionType</c> names another
///     exception.
/// </summary>
[PublicAPI]
[SuppressMessage("Roslynator", "RCS1194:Implement exception constructors", Justification = "The exception always carries the error that caused it.")]
public sealed class DomainPrimitiveValidationException : ArgumentException
{
    /// <summary>
    ///     Creates the exception.
    /// </summary>
    /// <param name="error">Why the value was rejected.</param>
    public DomainPrimitiveValidationException(PrimitiveError error) : this(error, "value") { }

    /// <summary>
    ///     Creates the exception.
    /// </summary>
    /// <param name="error">Why the value was rejected.</param>
    /// <param name="paramName">The parameter that held the value.</param>
    public DomainPrimitiveValidationException(PrimitiveError error, string paramName) : base($"[{error.Code}] {error.Message}", paramName) => Error = error;

    /// <summary>
    ///     Creates the exception.
    /// </summary>
    /// <param name="error">Why the value was rejected.</param>
    /// <param name="innerException">What caused it.</param>
    public DomainPrimitiveValidationException(PrimitiveError error, Exception? innerException) : this(error, innerException, "value") { }

    /// <summary>
    ///     Creates the exception.
    /// </summary>
    /// <param name="error">Why the value was rejected.</param>
    /// <param name="innerException">What caused it.</param>
    /// <param name="paramName">The parameter that held the value.</param>
    public DomainPrimitiveValidationException(PrimitiveError error, Exception? innerException, string paramName)
        : base($"[{error.Code}] {error.Message}", paramName, innerException) => Error = error;

    /// <summary>
    ///     Why the value was rejected.
    /// </summary>
    public PrimitiveError Error { get; }
}
