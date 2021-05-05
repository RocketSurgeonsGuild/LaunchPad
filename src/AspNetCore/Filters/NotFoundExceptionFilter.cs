using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Foundation;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters
{
    /// <summary>
    /// Not found exception that catches not found messages that might have been thrown by calling code.
    /// </summary>
    class NotFoundExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Create a new NotFoundExceptionFilter
        /// </summary>
        /// <param name="problemDetailsFactory"></param>
        public NotFoundExceptionFilter(ProblemDetailsFactory problemDetailsFactory) => _problemDetailsFactory = problemDetailsFactory;

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (context?.Exception is NotFoundException exception)
            {
                context.ExceptionHandled = true;
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    context.HttpContext,
                    StatusCodes.Status404NotFound,
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

                context.Result = new ObjectResult(problemDetails) {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
        }

        /// <inheritdoc />
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }
    }
}
