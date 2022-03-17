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
        if (actionContext.ActionDescriptor.Properties.TryGetValue(typeof(CustomizeValidatorAttribute), out var value)
         && value is string[] includeProperties)
        {
            result = new ValidationResult(
                result.Errors
                      .Join(includeProperties, z => z.PropertyName, z => z, (a, b) => a, StringComparer.OrdinalIgnoreCase)
            );
        }

        if (result.IsValid) return result;

        actionContext.HttpContext.Items[typeof(ValidationResult)] = result;
        return result;
    }
}
