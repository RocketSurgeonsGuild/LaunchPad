using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Rocket.Surgery.LaunchPad.Primitives;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     Not found exception that catches not found messages that might have been thrown by calling code.
/// </summary>
/// <remarks>
///     Create a new NotFoundExceptionFilter
/// </remarks>
/// <param name="problemDetailsFactory"></param>
internal class NotFoundExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : ProblemDetailsExceptionFilter<NotFoundException>(StatusCodes.Status404NotFound, problemDetailsFactory)
{
}
