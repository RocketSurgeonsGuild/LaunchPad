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
        return result;
    }
}
