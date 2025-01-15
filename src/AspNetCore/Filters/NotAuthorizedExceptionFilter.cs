using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     Not authorized exception that catches not authorized messages that might have been thrown by calling code.
/// </summary>
/// <remarks>
///     Not authorized exception that catches not authorized messages that might have been thrown by calling code.
/// </remarks>
internal class NotAuthorizedExceptionFilter
    (ProblemDetailsFactory problemDetailsFactory) : ProblemDetailsExceptionFilter<NotAuthorizedException>(StatusCodes.Status403Forbidden, problemDetailsFactory)
{ }
