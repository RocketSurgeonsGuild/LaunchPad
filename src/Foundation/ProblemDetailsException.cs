namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     NotFoundException.
/// </summary>
/// <seealso cref="System.Exception" />
[PublicAPI]
public abstract class ProblemDetailsException : Exception, IProblemDetailsData
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProblemDetailsException" /> class.
    /// </summary>
    private ProblemDetailsException() : this("An problem occurred.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProblemDetailsException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ProblemDetailsException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProblemDetailsException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is
    ///     specified.
    /// </param>
    protected ProblemDetailsException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Additional properties
    /// </summary>
    public IDictionary<string, object?> Properties { get; init; } = new Dictionary<string, object?>(StringComparer.Ordinal);

    /// <summary>
    ///     Request title
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    ///     Request Type
    /// </summary>
    public string? Link { get; init; }

    /// <summary>
    ///     The instance for the request
    /// </summary>
    public string? Instance { get; init; }
}
