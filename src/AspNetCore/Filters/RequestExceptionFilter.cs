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
    public class RequestExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Not found exception that catches not found messages that might have been thrown by calling code.
        /// </summary>
        public RequestExceptionFilter(ProblemDetailsFactory problemDetailsFactory) => _problemDetailsFactory = problemDetailsFactory;

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (context?.Exception is RequestException)
            {
                context.ExceptionHandled = true;
                context.Result = new BadRequestObjectResult(_problemDetailsFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status400BadRequest, detail: context.Exception.Message));
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
