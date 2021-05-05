using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Threading.Tasks;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters
{
    /// <summary>
    /// Not found exception that catches not found messages that might have been thrown by calling code.
    /// </summary>
    class RequestFailedExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Not found exception that catches not found messages that might have been thrown by calling code.
        /// </summary>
        public RequestFailedExceptionFilter(ProblemDetailsFactory problemDetailsFactory) => _problemDetailsFactory = problemDetailsFactory;

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is RequestFailedException exception)
            {
                context.ExceptionHandled = true;
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    context.HttpContext,
                    StatusCodes.Status400BadRequest,
                    detail: exception.Message,
                    title: exception.Title,
                    type: exception.Link,
                    instance: exception.Instance
                );

                foreach (var item in exception.Properties)
                {
                    if (problemDetails.Extensions.ContainsKey(item.Key))
                        continue;
                    problemDetails.Extensions.Add(item);
                }

                context.Result = new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status400BadRequest
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