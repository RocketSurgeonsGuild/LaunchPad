using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rocket.Surgery.LaunchPad.Extensions;

// ReSharper disable ClassNeverInstantiated.Global

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters
{
    /// <summary>
    /// Not found exception that catches not found messages that might have been thrown by calling code.
    /// </summary>
    public class NotFoundExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (context?.Exception is NotFoundException)
            {
                context.ExceptionHandled = true;
                context.Result = new NotFoundResult();
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
