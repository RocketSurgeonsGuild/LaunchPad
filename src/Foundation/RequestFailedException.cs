using System;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     An exception for failed requests, with overloads to take additional details
/// </summary>
/// <remarks>
///     This data can then be used to surface information to the user through something like problem details.
/// </remarks>
/// <seealso cref="System.Exception" />
public class RequestFailedException : Exception, IProblemDetailsData
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestFailedException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RequestFailedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestFailedException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is
    ///     specified.
    /// </param>
    public RequestFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestFailedException" /> class.
    /// </summary>
    public RequestFailedException() : this(string.Empty)
    {
    }

    /// <summary>
    ///     Additional properties
    /// </summary>
    public IDictionary<string, object> Properties { get; init; } = new Dictionary<string, object>(StringComparer.Ordinal);

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
