using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation
{
    internal class ValidatorInterceptor : IValidatorInterceptor
    {
        public IValidationContext BeforeMvcValidation(
            ControllerContext controllerContext,
            IValidationContext validationContext
        ) => validationContext;

        public ValidationResult AfterMvcValidation(
            ControllerContext controllerContext,
            IValidationContext validationContext,
            ValidationResult result
        )
        {
            controllerContext.HttpContext.Items[typeof(ValidationResult)] = result;
            return result;
        }
    }
}