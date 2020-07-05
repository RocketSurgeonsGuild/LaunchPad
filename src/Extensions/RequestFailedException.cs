using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// An exception for failed requests, with overloads to take additional details
    /// </summary>
    /// <remarks>
    /// This data can then be used to surface information to the user through something like problem details.
    /// </remarks>
    /// <seealso cref="System.Exception" />
    public class RequestFailedException : Exception
    {
        /// <summary>
        /// Additional properties
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Request title
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// Request Type
        /// </summary>
        public string? Link { get; }

        /// <summary>
        /// The instance for the request
        /// </summary>
        public string? Instance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="properties">The properties to attach to the exception</param>
        /// <param name="title"></param>
        /// <param name="link"></param>
        /// <param name="instance"></param>
        public RequestFailedException(
            string message,
            string? title = null,
            string? link = null,
            string? instance = null,
            IDictionary<string, object>? properties = null
        ) : base(message)
        {
            Properties = properties ?? new Dictionary<string, object>(StringComparer.Ordinal);
            Title = title;
            Link = link;
            Instance = instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFailedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="properties">The properties to attach to the exception</param>
        /// <param name="title"></param>
        /// <param name="link"></param>
        /// <param name="instance"></param>
        public RequestFailedException(
            string message,
            Exception innerException,
            string? title = null,
            string? link = null,
            string? instance = null,
            IDictionary<string, object>? properties = null
        ) : base(message, innerException)
        {
            Properties = properties ?? new Dictionary<string, object>(StringComparer.Ordinal);
            Title = title;
            Link = link;
            Instance = instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFailedException"/> class.
        /// </summary>
        public RequestFailedException() : this(string.Empty) { }
    }
}