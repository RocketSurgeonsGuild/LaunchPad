using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Rocket.Surgery.LaunchPad.Primitives;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     Request failed exception that catches issues that might have been thrown by calling code.
/// </summary>
/// <remarks>
///     Request failed exception that catches issues that might have been thrown by calling code.
/// </remarks>
internal class RequestFailedExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : ProblemDetailsExceptionFilter<RequestFailedException>(StatusCodes.Status400BadRequest, problemDetailsFactory)
{
}
