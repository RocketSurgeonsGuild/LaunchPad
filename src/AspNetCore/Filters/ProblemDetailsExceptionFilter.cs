using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

/// <summary>
///     An exception filter that catches the given problem details exception and uses the given status code to return the result
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public abstract class ProblemDetailsExceptionFilter<T> : IExceptionFilter, IAsyncExceptionFilter
    where T : Exception, IProblemDetailsData
{
    private readonly int _statusCode;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    /// <summary>
    ///     Create the problem details filter
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="problemDetailsFactory"></param>
    protected ProblemDetailsExceptionFilter(int statusCode, ProblemDetailsFactory problemDetailsFactory)
    {
        _statusCode = statusCode;
        _problemDetailsFactory = problemDetailsFactory;
    }

    /// <summary>
    ///     Allows changing the problem details before it is returned
    /// </summary>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    protected virtual ProblemDetails CustomizeProblemDetails(ProblemDetails problemDetails)
    {
        return problemDetails;
    }

    /// <inheritdoc />
    public Task OnExceptionAsync(ExceptionContext context)
    {
        OnException(context);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is T exception)
        {
            context.ExceptionHandled = true;
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                _statusCode,
                detail: exception.Message,
                title: exception.Title,
                type: exception.Link,
                instance: exception.Instance
            );

            foreach (var item in exception.Properties)
            {
                if (problemDetails.Extensions.ContainsKey(item.Key)) continue;
                problemDetails.Extensions.Add(item);
            }

            problemDetails = CustomizeProblemDetails(problemDetails);
            context.Result = new ObjectResult(problemDetails) { StatusCode = _statusCode };
        }
    }
}
