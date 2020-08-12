using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation
{
#pragma warning disable 618
    internal class ValidatorInterceptor : IActionContextValidatorInterceptor
#pragma warning restore 618
    {
        public IValidationContext BeforeMvcValidation(ActionContext controllerContext, IValidationContext validationContext) => validationContext;

        public ValidationResult AfterMvcValidation(ActionContext controllerContext, IValidationContext validationContext, ValidationResult result)
        {
            controllerContext.HttpContext.Items[typeof(ValidationResult)] = result;
            return result;
        }
    }
}