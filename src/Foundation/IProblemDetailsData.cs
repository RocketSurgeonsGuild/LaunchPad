using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.Foundation
{
    public interface IProblemDetailsData
    {
        /// <summary>
        /// Additional properties
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Request title
        /// </summary>
        string? Title { get; }

        /// <summary>
        /// Request Type
        /// </summary>
        string? Link { get; }

        /// <summary>
        /// The instance for the request
        /// </summary>
        string? Instance { get; }
    }
}