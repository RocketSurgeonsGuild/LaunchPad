using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     Captures the validation exception
/// </summary>
public class ValidationExceptionFilter : IExceptionFilter, IActionFilter, IOrderedFilter
{
    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult { Value: ValidationProblemDetails vpd }
         && context.HttpContext.Items.TryGetValue(typeof(ValidationResult), out var result))
        {
            var r = (ValidationResult)result!;
            context.Result = new ObjectResult(FluentValidationProblemDetails.From(vpd, r))
            {
                StatusCode = vpd.Status,
            };
        }
    }

    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        if (!( context.Exception is ValidationException validationException )) return;
        context.ExceptionHandled = true;
        context.Result = new UnprocessableEntityObjectResult(new FluentValidationProblemDetails(validationException.Errors));
    }

    public int Order { get; } = -2000;
}
