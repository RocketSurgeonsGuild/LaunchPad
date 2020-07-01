using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Rocket.Surgery.AspNetCore.FluentValidation
{
    /// <summary>
    /// Captures the validation exception
    /// </summary>
    public class ValidationExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (!(context.Exception is ValidationException validationException)) return;
            context.ExceptionHandled = true;
            context.Result = new UnprocessableEntityObjectResult(new FluentValidationProblemDetails(validationException.Errors));
        }

        /// <inheritdoc />
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }
    }
}
