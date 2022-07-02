using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Foundation;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     Request failed exception that catches issues that might have been thrown by calling code.
/// </summary>
internal class RequestFailedExceptionFilter : ProblemDetailsExceptionFilter<RequestFailedException>
{
    /// <summary>
    ///     Request failed exception that catches issues that might have been thrown by calling code.
    /// </summary>
    public RequestFailedExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : base(StatusCodes.Status400BadRequest, problemDetailsFactory)
    {
    }
}
