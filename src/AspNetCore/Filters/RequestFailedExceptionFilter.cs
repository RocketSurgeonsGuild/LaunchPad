using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rocket.Surgery.LaunchPad.Extensions;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters
{
    /// <summary>
    /// Not found exception that catches not found messages that might have been thrown by calling code.
    /// </summary>
    public class RequestFailedExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Not found exception that catches not found messages that might have been thrown by calling code.
        /// </summary>
        public RequestFailedExceptionFilter(ProblemDetailsFactory problemDetailsFactory) => _problemDetailsFactory = problemDetailsFactory;

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (context?.Exception is RequestFailedException requestFailedException)
            {
                context.ExceptionHandled = true;
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    context.HttpContext,
                    StatusCodes.Status400BadRequest,
                    detail: requestFailedException.Message,
                    title: requestFailedException.Title,
                    type: requestFailedException.Link,
                    instance: requestFailedException.Instance
                );

                foreach (var item in requestFailedException.Properties)
                {
                    if (problemDetails.Extensions.ContainsKey(item.Key)) continue;
                    problemDetails.Extensions.Add(item);
                }

                context.Result = new BadRequestObjectResult(problemDetails);
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