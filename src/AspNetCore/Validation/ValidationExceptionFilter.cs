using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     Captures the validation exception
/// </summary>
public class ValidationExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
{
    /// <inheritdoc />
    public Task OnExceptionAsync(ExceptionContext context)
    {
        OnException(context);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        if (!( context.Exception is ValidationException validationException )) return;
        context.ExceptionHandled = true;
        context.Result = new UnprocessableEntityObjectResult(new FluentValidationProblemDetails(validationException.Errors));
    }
}
