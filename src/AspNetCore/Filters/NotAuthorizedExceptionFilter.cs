using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     Not authorized exception that catches not authorized messages that might have been thrown by calling code.
/// </summary>
internal class NotAuthorizedExceptionFilter : ProblemDetailsExceptionFilter<NotAuthorizedException>
{
    /// <summary>
    ///     Not authorized exception that catches not authorized messages that might have been thrown by calling code.
    /// </summary>
    public NotAuthorizedExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : base(StatusCodes.Status403Forbidden, problemDetailsFactory)
    {
    }
}
