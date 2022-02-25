using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

internal class ValidatorInterceptor : IValidatorInterceptor
{
    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext validationContext)
    {
        return validationContext;
    }

    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
    {
        actionContext.HttpContext.Items[typeof(ValidationResult)] = result;
        if (actionContext.ActionDescriptor.Properties.TryGetValue(typeof(CustomizeValidatorAttribute), out var value)
         && value is string[] includeProperties)
        {
            return new ValidationResult(
                result.Errors
                      .Join(includeProperties, z => z.PropertyName, z => z, (a, b) => a, StringComparer.OrdinalIgnoreCase)
            );
        }

        return result;
    }
}
