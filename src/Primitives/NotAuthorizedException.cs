namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     NotAuthorizedException.
/// </summary>
/// <seealso cref="Exception" />
[PublicAPI]
public class NotAuthorizedException : ProblemDetailsException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotAuthorizedException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotAuthorizedException(string message) : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotAuthorizedException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is
    ///     specified.
    /// </param>
    public NotAuthorizedException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotAuthorizedException" /> class.
    /// </summary>
    public NotAuthorizedException() : base("Not Authorized")
    { }
}
