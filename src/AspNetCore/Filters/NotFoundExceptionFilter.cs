using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Foundation;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     Not found exception that catches not found messages that might have been thrown by calling code.
/// </summary>
internal class NotFoundExceptionFilter : ProblemDetailsExceptionFilter<NotFoundException>
{
    /// <summary>
    ///     Create a new NotFoundExceptionFilter
    /// </summary>
    /// <param name="problemDetailsFactory"></param>
    public NotFoundExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : base(StatusCodes.Status404NotFound, problemDetailsFactory)
    {
    }
}
